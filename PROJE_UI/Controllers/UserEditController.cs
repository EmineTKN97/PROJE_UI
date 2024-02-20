using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;

namespace PROJE_UI.Controllers
{
    public class UserEditController : Controller
    {
        private readonly HttpClient _client;

        public UserEditController(HttpClient client)
        {
            _client = client;
        }
        [HttpGet]
        public async Task <IActionResult> Index()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var response = await _client.GetAsync($"https://localhost:7185/api/Blogs/GetByUserId?UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                return View(result);
            }
            else
            {
                return RedirectToAction("Error", new { message = "API ile iletişim sırasında bir hata oluştu." });
            }
        }
    }
}
