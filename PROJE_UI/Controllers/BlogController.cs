using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using PROJE_UI.Models;
using PROJE_UI.ViewModels;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            return View(new AddBlog());

        }

        [HttpPost]
        public async Task<IActionResult> Blog(AddBlog model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var userRole = HttpContext.Request.Cookies["UserRole"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "UserRegister");
            }
            model.UserId=Guid.Parse(userId);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"https://localhost:7185/api/Blogs/AddBlog?UserId={model.UserId}", content);
                var apiResponse = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index", "Home");
        }
    }


 }



