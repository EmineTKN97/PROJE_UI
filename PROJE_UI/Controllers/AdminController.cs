using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using NuGet.Protocol.Plugins;

namespace PROJE_UI.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiServiceOptions _apiServiceOptions;

        public AdminController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            var adminrole = HttpContext.Request.Cookies["AdminRole"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.GetAsync($"{BaseUrl}api/Blogs/GetAllBlogsDetails");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                if (result != null && result.Any())
                {
                    return View(result);
                }
                else
                {
                    ViewData["NoCommentsMessage"] = "HENÜZ HİÇ Blog YOK.";
                    return View();
                }

            }
            return View();
        }
        [HttpGet]
        public IActionResult LoginAdmin()
        {
            return View(new Admin());
        }
        [HttpPost]
        public async Task<IActionResult> LoginAdmin(Admin model)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var handler = new JwtSecurityTokenHandler();

            using (var response = await _client.PostAsync($"{BaseUrl}api/Auth/loginAdmin", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<LoginApiResponseModel>(apiResponse);
                    var token = handler.ReadJwtToken(data.Data.Token);
                    var adminIdClaim = token?.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                    var adminRoleClaim = token?.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                    var adminName = token?.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");

                    if (adminIdClaim != null && adminRoleClaim != null)
                    {
                        SetUserCookies(adminIdClaim.Value,adminRoleClaim.Value, data.Data.Token);

                    }
                    TempData["SuccessLogin"] = $"{data.Message} {adminName.Value.ToUpperInvariant()}";
                    return RedirectToAction("Index", "Admin");

                }
                else 
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    TempData["ErrorLogin"] = apiResponse;
                    return View();
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            var adminrole = HttpContext.Request.Cookies["AdminRole"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.GetAsync($"{BaseUrl}api/Users/GetAll");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<User>>(apiResponse);
                if (result != null && result.Any())
                {
                    return View(result);
                }
                else
                {
                    TempData["NotUser"] = "HENÜZ HİÇ KULLANICI YOK.";
                    return View();
                }

            }
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var adminResponse = await _client.GetAsync($"{BaseUrl}api/Admins/GetById?AdminId={adminId}");

            if (!adminResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", new { message = "Admin bulunamadı." });
            }

            var ApiResponse = await adminResponse.Content.ReadAsStringAsync();
            var adminResult = JsonConvert.DeserializeObject<Admin>(ApiResponse);
            return View(adminResult);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(Admin model)
        {
            var adminId = HttpContext.Request.Cookies["adminId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseUrl}api/Admins/UpdateAdmin?id={adminId}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessUpdateProfile"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                TempData["ErrorUpdateProfile"] = errorResponse;
                return RedirectToAction("Index", "Admin");
            }
        }
        [HttpGet]
        public IActionResult PasswordOperation()
        {
            return View(new AdminPasswordChange());
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(AdminPasswordChange model)
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseUrl}api/Admins/ChangeAdminPassword?currentPassword={model.currentPassword}&newPassword={model.newPassword}&AdminId={adminId}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessUpdatePassword"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                TempData["ErrorUpdatePassword"] = errorResponse;
                return RedirectToAction("Index", "Admin");
            }
        }
        [HttpGet]
        public async Task<IActionResult> DeleteAdmin()
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var adminResponse = await _client.GetAsync($"{BaseUrl}api/Admins/GetById?AdminId={adminId}");

            if (!adminResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", new { message = "Kullanıcı bulunamadı." });
            }

            var ApiResponse = await adminResponse.Content.ReadAsStringAsync();
            var adminResult = JsonConvert.DeserializeObject<Admin>(ApiResponse);
            return View(adminResult);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(Admin model)
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"{BaseUrl}api/Admins/DeleteAdmin?id={adminId}");

            if (response.IsSuccessStatusCode)
            {
                Response.Cookies.Delete("AdminId");
                Response.Cookies.Delete("AdminRole");
                Response.Cookies.Delete("Bearer");

                var ApiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessDeleteAdmin"] = ApiResponse;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var ApiResponse = await response.Content.ReadAsStringAsync();
                TempData["ErrorDeleteAdmin"] = ApiResponse;
                return View("Index", "Admin");
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAllComment()
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            var adminrole = HttpContext.Request.Cookies["AdminRole"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.GetAsync($"{BaseUrl}api/BlogComments/GetCommentsDetails");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<BlogComment>>(apiResponse);
                if (result != null && result.Any())
                {
                    return View(result);
                }
                else
                {
                    ViewData["NoCommentsMessage"] = "HENÜZ HİÇ YORUM YOK.";
                    return View();
                }

            }
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> DeleteBlogComment(Guid CommentId, Guid userId)
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"{BaseUrl}api/BlogComments/Delete?id={CommentId}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessDeleteBlogComment"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorDeleteBlogComment"] = errorResponse;
            return RedirectToAction("GetAllComment", "Admin");

        }
        [HttpGet]
        public async Task<IActionResult> AddTicketPrice()
        {
            var id = "e7c37d57-4a9b-44fb-90ae-d9a20937ea14";

            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            var adminrole = HttpContext.Request.Cookies["AdminRole"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.GetAsync($"{BaseUrl}api/Costs/GetById?id={id}");
            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", new { message = "Fiyat bulunamadı." });
            }

            var ApiResponse = await response.Content.ReadAsStringAsync();
            var Result = JsonConvert.DeserializeObject<Cost>(ApiResponse);
            return View(Result);
        }
        [HttpPost]
        public async Task<IActionResult> AddTicketPrice(Cost model)
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var adminRole = HttpContext.Request.Cookies["AdminRole"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(adminId) || string.IsNullOrEmpty(adminRole))
            {
                return RedirectToAction("Login", "User");
            }
        
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseUrl}api/Costs/UpdateTicketPrice?id={model.Id}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessAddPrice"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorAddPrice"] = errorResponse;
            return RedirectToAction("Index", "Admin");

        }
        public async Task<IActionResult> LogOut()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("AdminId");
            Response.Cookies.Delete("AdminRole");
            Response.Cookies.Delete("Bearer");
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        public async Task<IActionResult> ChangeRoles(Guid UserId)
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("LoginAdmin", "Admin");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.PostAsync($"{BaseUrl}api/Auth/ChangeRole?UserId={UserId}", null);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();

                TempData["SuccessUpdateRoles"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorUpdateRoles"] = errorResponse;
            return RedirectToAction("Index", "Admin");

        }
       
        public void SetUserCookies(string adminId, string adminRole, string bearerToken)
        {
            var userCookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(10)
            };
            Response.Cookies.Append("AdminId", adminId, userCookieOptions);
            Response.Cookies.Append("AdminRole", adminRole, userCookieOptions);
            Response.Cookies.Append("Bearer", bearerToken, userCookieOptions);

        }

    }
}
