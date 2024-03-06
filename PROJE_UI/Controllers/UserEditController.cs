using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;

namespace PROJE_UI.Controllers
{
    public class UserEditController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiServiceOptions _apiServiceOptions;

        public UserEditController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var response = await _client.GetAsync($"{BaseUrl}api/Blogs/GetByUserId?UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                return View(result);
            }
            else
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["ErrorUserData"] = apiResponse;
                return RedirectToAction("Register","User");
            }
        }
        [HttpGet]
        public async Task<IActionResult> Success()
        {
           
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Error()
        {
          
            return View();
        }
    }
}
