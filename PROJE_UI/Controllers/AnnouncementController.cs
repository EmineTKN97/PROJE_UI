using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using System.Net.Http.Headers;
using System.Text;

namespace PROJE_UI.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiServiceOptions _apiServiceOptions;

        public AnnouncementController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpGet]
        public IActionResult AddAnnouncement()
        {
            return View(new Announcement());
        }
        [HttpPost]
        public async Task<IActionResult> AddAnnouncement(Announcement model)
        {

            var adminId = HttpContext.Request.Cookies["AdminId"];
            var adminRole = HttpContext.Request.Cookies["AdminRole"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(adminId) || string.IsNullOrEmpty(adminRole))
            {
                return RedirectToAction("LoginAdmin", "Admin");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}api/Announcements/AddAnnouncement?adminId={adminId}", content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessAddAnnouncement"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponse);
                foreach (var validationError in errorModel.ValidationErrors)
                {
                    ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                }

                ViewBag.ErrorMessages = errorModel.ValidationErrors.Select(e => e.ErrorMessage).ToList();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ListAnnouncement()
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
        [HttpPost]
        public async Task<IActionResult> DeleteAnnouncement(Guid AnnouncementId)
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("LoginAdmin", "Admin");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"{BaseUrl}api/Announcements/DeleteAnnouncement?id={AnnouncementId}&adminİd={adminId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessDeleteAnnouncement"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorDeleteAnnouncement"] = errorResponse;
            return RedirectToAction("ListAnnouncement", "Announcement");

        }
        [HttpGet]
        public async Task<IActionResult> UpdateAnnouncement(Guid AnnouncementId)
        {
            var announcementResponse = await _client.GetAsync($"{BaseUrl}api/Announcements/GetById?id={AnnouncementId}");

            if (!announcementResponse.IsSuccessStatusCode)
            {
                var response = await announcementResponse.Content.ReadAsStringAsync();
                ViewData["ErrorGetAnnouncement"] = response;
                return View();
            }

            var ApiResponse = await announcementResponse.Content.ReadAsStringAsync();
            var announcementResult = JsonConvert.DeserializeObject<Announcement>(ApiResponse);
            return View(announcementResult);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAnnouncement(Announcement model)
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseUrl}api/Announcements/UpdateAnnouncement?id={model.AnnouncementId}&adminId={adminId}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessUpdateAnnouncement"] = apiResponse;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorModel = JsonConvert.DeserializeObject<ApiErrorResponse>(errorResponse);
                foreach (var validationError in errorModel.ValidationErrors)
                {
                    ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                }

                ViewBag.ErrorMessages = errorModel.ValidationErrors.Select(e => e.ErrorMessage).ToList();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return View(model);

        }

    }
}


