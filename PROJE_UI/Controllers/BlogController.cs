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
                return RedirectToAction("Login", "User");
            }
            model.UserId=Guid.Parse(userId);
            model.BlogDate=DateTime.Now;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"https://localhost:7185/api/Blogs/AddBlog?UserId={model.UserId}", content);
            var apiResponse = await response.Content.ReadAsStringAsync();
            return RedirectToAction("Index", "UserEdit");
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteBlog(Guid blogId)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var userRole = HttpContext.Request.Cookies["UserRole"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            
            var response = await _client.DeleteAsync($"https://localhost:7185/api/Blogs/DeleteBlog?id={blogId}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                // Başarılı bir şekilde silinirse, kullanıcıyı yönlendir
                return RedirectToAction("Index", "UserEdit");
            }
            else
            {
                // Silme işlemi başarısız olursa, hata mesajını görüntüle veya başka bir işlem yap
                var apiErrorResponse = await response.Content.ReadAsStringAsync();
                return View("Error");
            }
        }
    }


 }



