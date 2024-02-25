﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using System.Net.Http.Headers;

namespace PROJE_UI.Controllers
{
    public class BlogMediaController : Controller
    {
        private readonly HttpClient _client;

        public BlogMediaController(HttpClient client)
        {
            _client = client;
        }
        [HttpGet]
        public async Task<IActionResult> AddBlogMedia()
        {
            return View(new BlogMedia());
        }

        [HttpPost]
        public async Task<IActionResult> AddBlogMedia(BlogMedia model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            using (var formContent = new MultipartFormDataContent())
            {

                formContent.Add(new StringContent(model.ImagePath.FileName), "ImagePath");
                formContent.Add(new StreamContent(model.ImagePath.OpenReadStream())
                {
                    Headers =
                {
                    ContentLength = model.ImagePath.Length,
                    ContentType= new MediaTypeHeaderValue(model.ImagePath.ContentType)

                }
                }, "file", model.ImagePath.FileName);
                var response = await _client.PostAsync($"https://localhost:7185/api/Medias/AddBlogMedia?BlogId={model.BlogId}&UserId{userId}", formContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "UserEdit");
                }
            }
            return View("Error");

        }
        [HttpPost]
        public async Task<IActionResult> UpdateBlogMedia(BlogMedia model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            if (model.ImagePath != null)
            {
                using (var formContent = new MultipartFormDataContent())
                {

                    formContent.Add(new StringContent(model.ImagePath.FileName), "ImagePath");
                    formContent.Add(new StreamContent(model.ImagePath.OpenReadStream())
                    {
                        Headers =
                    {
                    ContentLength = model.ImagePath.Length,
                    ContentType= new MediaTypeHeaderValue(model.ImagePath.ContentType)

                    }
                    }, "file", model.ImagePath.FileName);

                    var response = await _client.PostAsync($"https://localhost:7185/api/Medias/UpdateBlogMedia?BlogId={model.BlogId}&UserId={userId}", formContent);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "UserEdit");
                    }
                }
            }

            return View("Error");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBlogMedia(Guid MediaId, Guid BlogId)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
   
            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"https://localhost:7185/api/Medias/DeleteBlogMedia?MediaId={MediaId}&UserId={userId}&BlogId={BlogId}");

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