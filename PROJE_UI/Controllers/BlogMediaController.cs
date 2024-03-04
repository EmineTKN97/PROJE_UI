using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using NuGet.Protocol.Plugins;
using PROJE_UI.Models;
using PROJE_UI.ViewModels;
using System.Net.Http.Headers;

namespace PROJE_UI.Controllers
{
    public class BlogMediaController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiServiceOptions _apiServiceOptions;

        public BlogMediaController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpGet]
        public async Task<IActionResult> AddBlogMedia()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var blogResponse = await _client.GetAsync($"{BaseUrl}api/Blogs/GetLatestBlogByUserId?UserId={userId}");

            if (!blogResponse.IsSuccessStatusCode)
            {

                return RedirectToAction("Error", new { message = "Blog bulunamadı." });
            }
            var ApiResponse = await blogResponse.Content.ReadAsStringAsync();
            var blogResult = JsonConvert.DeserializeObject<AddBlog>(ApiResponse);
            return View(blogResult);
        }

        [HttpPost]
        public async Task<IActionResult> AddBlogMedia(BlogMedia model, Guid BlogId)
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
                var response = await _client.PostAsync($"{BaseUrl}api/Medias/AddBlogMedia?BlogId={BlogId}&UserId={userId}", formContent);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index","UserEdit");
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

                    var response = await _client.PutAsync($"{BaseUrl}api/Medias/UpdateBlogMedia?BlogId={model.BlogId}&UserId={userId}", formContent);
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
            var response = await _client.DeleteAsync($"{BaseUrl}api/Medias/DeleteBlogMedia?MediaId={MediaId}&UserId={userId}&BlogId={BlogId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessDeleteBlogMedia"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorDeleteBlogMedia"] = errorResponse;
            return RedirectToAction("Index", "UserEdit");

        }

    }
}
