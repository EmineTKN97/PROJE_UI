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

        public BlogCommentController(HttpClient client)
        {
            _client = client;
        }
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
            var response = await _client.PostAsync($"https://localhost:7185/api/BlogComments/AddComment?blogId={blogId}&UserId={userId}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "UserEdit");
            }
            return RedirectToAction("BlogDetails","BlogDetails");
        }
        [HttpGet]
        public async Task<IActionResult> UserAllComment()
        {
            return View();
        }
    }
}