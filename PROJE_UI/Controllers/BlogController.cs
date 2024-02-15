using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using PROJE_UI.ViewModels;
using System.Net.Http;
using System.Text;

namespace PROJE_UI.Controllers
{
    public class BlogController : Controller
    {
        private readonly HttpClient _client;

        public BlogController(HttpClient client)
        {
            _client = client;
        }
        [HttpGet]
        public IActionResult Blog()
        {
            return View(new Blog());
        }

        [HttpPost]
        public async Task<IActionResult> Blog(Blog model)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
            
                return RedirectToAction("Login", "UserRegister");
            }

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync("https://localhost:7185/api/Blogs/AddBlog", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var addBlog = JsonConvert.DeserializeObject<Blog>(apiResponse);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Hata durumuyla başa çıkın
                    ModelState.AddModelError("", "Blog kaydedilemedi. Lütfen tekrar deneyin.");
                    return View(model);
                }
            }
        }

    }
}

