using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;

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
        public IActionResult Index()
        {
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
                    ViewData["NotUser"] = "HENÜZ HİÇ Kullanıcı YOK.";
                    return View();
                }

            }
                return View();
            
        }
    }
}
