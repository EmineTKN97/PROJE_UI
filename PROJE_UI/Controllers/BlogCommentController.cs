using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using PROJE_UI.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PROJE_UI.Controllers
{
    public class BlogCommentController : Controller
    {
        private readonly HttpClient _client;
       private readonly  ApiServiceOptions _apiServiceOptions;

        public BlogCommentController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpPost]
        public async Task<IActionResult> AddComment(BlogComment model)
        { 
            var userId = HttpContext.Request.Cookies["UserId"];
            var userRole = HttpContext.Request.Cookies["UserRole"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var blogId = model.BlogId;
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync($"{BaseUrl}api/BlogComments/AddComment?blogId={blogId}&UserId={userId}", content);

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessAddComment"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorAddComment"] = errorResponse;
            return RedirectToAction("Index", "UserEdit");
        }
        [HttpGet]
        public async Task<IActionResult> UserAllComment()
        {

            var userId = HttpContext.Request.Cookies["UserId"];
            var response = await _client.GetAsync($"{BaseUrl}api/BlogComments/GetByUserId?UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<BlogComment>>(apiResponse);
                if (result != null && result.Any())
                {
                    return View(result);
                }
                else
                {
                    ViewData["NoCommentsMessage"] = "HENÜZ HİÇ YORUMUNUZ YOK.";
                    return View();
                }
              
            }
            else
            {
                return RedirectToAction("Error", new { message = "API ile iletişim sırasında bir hata oluştu." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid CommentId, Guid userId)
        {
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.DeleteAsync($"{BaseUrl}api/BlogComments/Delete?id={CommentId}&UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                TempData["SuccessDeleteComment"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorDeleteComment"] = errorResponse;
            return RedirectToAction("UserAllComment", "BlogComment");

        }
        [HttpGet]
        public async Task<IActionResult> UpdateBlogComment(Guid CommentId)
        {
            var blogResponse = await _client.GetAsync($"{BaseUrl}api/BlogComments/GetById?CommentId={CommentId}");

            if (!blogResponse.IsSuccessStatusCode)
            {
                return RedirectToAction("Error", new { message = "Blog bulunamadı." });
            }

            var ApiResponse = await blogResponse.Content.ReadAsStringAsync();
            var blogResult = JsonConvert.DeserializeObject<BlogComment>(ApiResponse);
            return View(blogResult);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBlogComment(BlogComment model)
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];

            if (string.IsNullOrEmpty(bearerToken))
            {
                return RedirectToAction("Login", "User");
            }
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"{BaseUrl}api/BlogComments/UpdateBlogComment?id={model.CommentId}&UserId={userId}", content);
            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();

                TempData["SuccessUpdateComment"] = apiResponse;
                return RedirectToAction("Index", "UserEdit");
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorUpdateComment"] = errorResponse;
            return RedirectToAction("UpdateBlogComment", "BlogComment");

        }
    }
}