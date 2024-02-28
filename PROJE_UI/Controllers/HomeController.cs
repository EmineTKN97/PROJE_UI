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
        private readonly ApiServiceOptions _apiServiceOptions;

        public HomeController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            return View();

        }
        [HttpGet]
        public async Task<IActionResult> AnnouncementDetails(Guid id)
        {
            var response = await _client.GetAsync($"{BaseUrl}api/Announcements/GetById?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Announcement>(apiResponse);
                return View(result);
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                TempData["ErrorAnnouncementDetails"] = errorResponse;
                return View();

            }
        }
        [HttpGet]
        public async Task<IActionResult> AllAnnouncementDetails()
        {
            var response = await _client.GetAsync($"{BaseUrl}api/Announcements/GetAllAnnouncement");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Announcement>>(apiResponse);
                return View(result);
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                TempData["ErrorAllAnnouncementDetails"] = errorResponse;
                return View();
            }
        }




    }
}

