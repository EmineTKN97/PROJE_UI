using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using System.Net.Http;
using System.Text;

namespace PROJE_UI.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly HttpClient _client;

        public HomeController(HttpClient client)
        {
            _client = client;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            return View();

        }
        [HttpGet]
        public async Task<IActionResult> AnnouncementDetails(Guid id)
        {
            var response = await _client.GetAsync($"https://localhost:7185/api/Announcements/GetById?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Announcement>(apiResponse);
                return View(result);
            }
            else
            {
                return RedirectToAction("Error", new { message = "API ile iletişim sırasında bir hata oluştu." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AllAnnouncementDetails()
        {
            var response = await _client.GetAsync("https://localhost:7185/api/Announcements/GetAllAnnouncement");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Announcement>>(apiResponse);
                return View(result);
            }
            else
            {
                return RedirectToAction("Error", new { message = "API ile iletişim sırasında bir hata oluştu." });
            }
        }




    }
}

