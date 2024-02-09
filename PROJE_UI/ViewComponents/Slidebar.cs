using Microsoft.AspNetCore.Mvc;
using PROJE_UI.Models;

namespace PROJE_UI.ViewComponents
{
    public class Slidebar : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public Slidebar(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var httpClient = _httpClientFactory.CreateClient(); // HttpClientFactory'den HTTP isteği için bir HttpClient oluşturun

            httpClient.BaseAddress = new Uri("https://localhost:7185/"); // API'nin base adresini burada belirtin

            var response = await httpClient.GetAsync("api/Blogs/GetLatestBlog"); // API endpoint'inizi ve route'ünüzü buraya ekleyin

            if (response.IsSuccessStatusCode)
            {
                var latestBlogs = await response.Content.ReadFromJsonAsync<List<Blog>>(); // API'den gelen veriyi okuyun
                return View(latestBlogs);
            }
            else
            {
                // Hata durumu ile başa çıkın
                return View(new List<Blog>()); // veya hata mesajını kullanıcıya göstermek için RedirectToAction("Error", new { message = "API ile iletişim sırasında bir hata oluştu." });
            }
        }
    }
}