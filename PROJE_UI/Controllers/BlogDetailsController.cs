using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using PROJE_UI.ViewModels;
using System.Text;

namespace PROJE_UI.Controllers
{
    public class BlogDetailsController : Controller
    {
            private readonly HttpClient _client;

            public BlogDetailsController(HttpClient client)
            {
                _client = client;
            }

            [HttpGet]
            public async Task<IActionResult> ListBlog()
            {
                var response = await _client.GetAsync("https://localhost:7185/api/Blogs/GetBlogsByCommentAndLikeCount");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiResponseModel<List<Blog>>>(apiResponse);

                    if (result.Success)
                    {
                        return View(result.Data);
                    }
                    else
                    {
                        return RedirectToAction("Error", new { message = result.Message });
                    }
                }
                else
                {
                    return RedirectToAction("Error", new { message = "API ile iletişim sırasında bir hata oluştu." });
                }
            }

            [HttpGet]
            public async Task<IActionResult> BlogDetails(Guid blogId)
            {
                var blogResponse = await _client.GetAsync($"https://localhost:7185/api/Blogs/GetById?id={blogId}");

                if (!blogResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Error", new { message = "Blog bulunamadı." });
                }

                var blogApiResponse = await blogResponse.Content.ReadAsStringAsync();
                var blogResult = JsonConvert.DeserializeObject<Blog>(blogApiResponse);

                if (blogResult == null)
                {
                    return RedirectToAction("Error", new { message = "Blog bulunamadı." });
                }

                var commentResponse = await _client.GetAsync($"https://localhost:7185/api/BlogComments/GetCommentsByBlogId?BlogId={blogId}");

                if (!commentResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Error", new { message = "API ile iletişim sırasında bir hata oluştu." });
                }

                var commentApiResponse = await commentResponse.Content.ReadAsStringAsync();
                var commentResult = JsonConvert.DeserializeObject<List<BlogComment>>(commentApiResponse);

                // BlogViewModel oluşturulur ve gerekli alanlar doldurulur
                var blogViewModel = new BlogViewModel
                {
                    Blogs = blogResult,
                    Comments = commentResult
                };

                return View(blogViewModel);
            }

            [HttpGet]
            public IActionResult AddBlogComment(Guid blogId)
            {
                var viewModel = new BlogViewModel
                {
                    BlogId = blogId,
                    NewComment = new BlogComment()
                };

                return View(viewModel);
            }
        [HttpPost]
        public async Task<IActionResult> AddBlogComment(BlogViewModel model)
        {
            model.NewComment.BlogId = model.BlogId;
            StringContent content = new StringContent(JsonConvert.SerializeObject(model.NewComment), Encoding.UTF8, "application/json");

            using (var response = await _client.PostAsync("https://localhost:7185/api/BlogComments/AddComment", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var addBlogComment = JsonConvert.DeserializeObject<BlogComment>(apiResponse);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Blog Yorumu kaydedilemedi. Lütfen tekrar deneyin.");
                    return View(model);
                }
            }
        }
    }
}

