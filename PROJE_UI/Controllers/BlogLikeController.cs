using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using PROJE_UI.ViewModels;
using System.Net.Http.Headers;
using System.Text;

namespace PROJE_UI.Controllers
{
    public class BlogLikeController : Controller
    {
        private readonly HttpClient _client;
        private readonly ApiServiceOptions _apiServiceOptions;

        public BlogLikeController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }

        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpPost]
        public async Task<IActionResult> AddLike(Like model, Guid BlogId)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var userRole = HttpContext.Request.Cookies["UserRole"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "User");
            }
            model.LikeDate = DateTime.Now;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}api/BlogLikes/AddBlogLike?blogId={BlogId}&UserId={userId}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessLike"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }
            var errorapiResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorLike"] =errorapiResponse;
            return RedirectToAction("Index", "UserEdit");
        }
        public async Task<IActionResult> Delete(Guid LikeId)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {

                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"{BaseUrl}api/BlogLikes/Delete?id={LikeId}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessDeleteLike"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorDeleteLike"] = errorResponse;
            return RedirectToAction("Index", "UserEdit");

        }
        [HttpGet]
        public async Task<IActionResult> ListBlogUserLike()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var response = await _client.GetAsync($"{BaseUrl}api/BlogLikes/GetLikesByBlogsByUserId?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<Blog>>(apiResponse);
                return View(result);

            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorListBlog"] = errorResponse;
            return RedirectToAction("ListBlog", "BlogDetails");

        }

    }
}

