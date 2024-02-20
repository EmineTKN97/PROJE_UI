using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

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
                    return RedirectToAction("Index", "UserEdit");
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

