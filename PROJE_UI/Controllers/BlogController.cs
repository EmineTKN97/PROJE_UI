using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
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
            return RedirectToAction("AddBlogMedia", "BlogMedia");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid blogId, Guid userId)
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"https://localhost:7185/api/Blogs/DeleteBlog?id={blogId}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "UserEdit");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> UpdateBlog(Guid blogId)
        {
            var blogResponse = await _client.GetAsync($"https://localhost:7185/api/Blogs/GetById?id={blogId}");

            if (!blogResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", new { message = "Blog bulunamadı." });
            }

            var ApiResponse = await blogResponse.Content.ReadAsStringAsync();
            var blogResult = JsonConvert.DeserializeObject<AddBlog>(ApiResponse);
            return View(blogResult);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBlog(AddBlog model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"https://localhost:7185/api/Blogs/UpdateBlog?id={model.BlogId}&UserId={userId}",content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "UserEdit");
            }
            else
            {
                return View("Error");
            }
        }
    }
 }



