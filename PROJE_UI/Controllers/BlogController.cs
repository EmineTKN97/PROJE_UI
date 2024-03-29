﻿using Microsoft.AspNetCore.Authorization;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PROJE_UI.Controllers
{
    public class BlogController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiServiceOptions _apiServiceOptions;

        public BlogController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions; 
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
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
            model.UserId = Guid.Parse(userId);
            model.BlogDate = DateTime.Now;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}api/Blogs/AddBlog?UserId={model.UserId}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessAddBlog"] = apiResponse;
                return RedirectToAction("AddBlogMedia", "BlogMedia");
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
        [HttpPost]
        public async Task<IActionResult> Delete(Guid blogId, Guid userId)
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {

                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"{BaseUrl}api/Blogs/DeleteBlog?id={blogId}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessDeleteBlog"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorDeletelog"] = errorResponse;
            return RedirectToAction("Index", "UserEdit");

        }
        [HttpGet]
        public async Task<IActionResult> UpdateBlog(Guid blogId)
        {
            var blogResponse = await _client.GetAsync($"{BaseUrl}api/Blogs/GetById?id={blogId}");

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
            var response = await _client.PutAsync($"{BaseUrl}api/Blogs/UpdateBlog?id={model.BlogId}&UserId={userId}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessUpdateBlog"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorUpdateBlog"] = errorResponse;
            return RedirectToAction("Index", "UserEdit");
        }
    }
}




