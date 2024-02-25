using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using PROJE_UI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using PROJE_UI.ViewModels;

namespace PROJE_UI.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _client;

        public UserController(HttpClient client)
        {
            _client = client;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
           
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var handler = new JwtSecurityTokenHandler();
            using (var response = await _client.PostAsync("https://localhost:7185/api/Auth/registerUser", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<Models.TokenOptions>(apiResponse);
                    var token = handler.ReadJwtToken(data.Token);
                    var userIdClaim = token?.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                    var userRoleClaim = token?.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                    if (userIdClaim != null && userRoleClaim != null)
                    {
                        SetUserCookies(userIdClaim.Value, userRoleClaim.Value, data.Token);
                    }
                    return RedirectToAction("AddPicture", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı kaydedilemedi. Lütfen tekrar deneyin.");
                    return View(model);
                }
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new User());
        }
        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var handler = new JwtSecurityTokenHandler();

            using (var response = await _client.PostAsync("https://localhost:7185/api/Auth/loginUser", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<TokenOptions>(apiResponse);
                    var token = handler.ReadJwtToken(data.Token);
                    var userIdClaim = token?.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                    var userRoleClaim = token?.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

                    if (userIdClaim != null && userRoleClaim != null)
                    {
                        SetUserCookies(userIdClaim.Value, userRoleClaim.Value, data.Token);
                    }
                    return RedirectToAction("Index", "UserEdit");
                }
                else
                {
                    ModelState.AddModelError("", "Giriş Yapılamadı. Lütfen tekrar deneyin.");
                    return View(model);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var userResponse = await _client.GetAsync($"https://localhost:7185/api/Users/GetById?UserId={userId}");

            if (!userResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", new { message = "Kullanıcı bulunamadı." });
            }

            var ApiResponse = await userResponse.Content.ReadAsStringAsync();
            var userResult = JsonConvert.DeserializeObject<User>(ApiResponse);
            return View(userResult);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"https://localhost:7185/api/Users/UpdateUser?id={userId}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "UserEdit");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public IActionResult PasswordOperation()
        {
            return View(new UserPaswordChange());
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UserPaswordChange model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            model.UserId = Guid.Parse(userId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"https://localhost:7185/api/Users/ChangePassword?currentPassword={model.currentPassword}&newPassword={model.newPassword}&UserId={userId}", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "UserEdit");
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePicture(UserMedia model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            model.UserId = Guid.Parse(userId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            if (model.ImagePath != null)
            {
                using (var formContent = new MultipartFormDataContent())
                {

                    formContent.Add(new StringContent(model.ImagePath.FileName), "ImagePath");
                    formContent.Add(new StreamContent(model.ImagePath.OpenReadStream())
                    {
                        Headers =
                {
                    ContentLength = model.ImagePath.Length,
                    ContentType= new MediaTypeHeaderValue(model.ImagePath.ContentType)

                }
                    }, "file", model.ImagePath.FileName);

                    var response = await _client.PostAsync($"https://localhost:7185/api/Medias/UpdateMedia?UserId={model.UserId}", formContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "UserEdit");
                    }
                }
            }

            return View("Error");
        }
        [HttpGet]
        public async Task<IActionResult> AddPicture()
        {
            return View(new UserMedia());
        }

        [HttpPost]
        public async Task<IActionResult> AddPicture(UserMedia model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            model.UserId = Guid.Parse(userId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            using (var formContent = new MultipartFormDataContent())
            {

                formContent.Add(new StringContent(model.ImagePath.FileName), "ImagePath");
                formContent.Add(new StreamContent(model.ImagePath.OpenReadStream())
                {
                    Headers =
                {
                    ContentLength = model.ImagePath.Length,
                    ContentType= new MediaTypeHeaderValue(model.ImagePath.ContentType)

                }
                }, "file", model.ImagePath.FileName);

                var response = await _client.PostAsync($"https://localhost:7185/api/Medias/AddUserMedia?UserId={model.UserId}", formContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "UserEdit");
                }
            }


            return View("Error");
        }
        [HttpPost]
        public async Task<IActionResult> DeletePicture(Guid userId, Guid MediaId)
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"https://localhost:7185/api/Medias/DeleteMedia?MediaId={MediaId}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "UserEdit");
            }
            else
            {
                return View("Error");
            }
        }
        public async Task<IActionResult> LogOut()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("UserId");
            Response.Cookies.Delete("UserRole");
            Response.Cookies.Delete("Bearer");
            return RedirectToAction("Index", "Home");

        }
        [HttpGet]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var userResponse = await _client.GetAsync($"https://localhost:7185/api/Users/GetById?UserId={userId}");

            if (!userResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", new { message = "Kullanıcı bulunamadı." });
            }

            var ApiResponse = await userResponse.Content.ReadAsStringAsync();
            var userResult = JsonConvert.DeserializeObject<User>(ApiResponse);
            return View(userResult);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(User model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"https://localhost:7185/api/Users/DeleteUser?id={userId}");

            if (response.IsSuccessStatusCode)
            {
                Response.Cookies.Delete("UserId");
                Response.Cookies.Delete("UserRole");
                Response.Cookies.Delete("Bearer");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View("Error");
            }
        }
        public void SetUserCookies(string userId, string userRole, string bearerToken)
        {
            var userCookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(10)
            };
            Response.Cookies.Append("UserId", userId, userCookieOptions);
            Response.Cookies.Append("UserRole", userRole, userCookieOptions);
            Response.Cookies.Append("Bearer", bearerToken, userCookieOptions);
        }
    }
}

