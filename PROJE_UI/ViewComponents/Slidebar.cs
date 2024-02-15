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
            var httpClient = _httpClientFactory.CreateClient(); 

            httpClient.BaseAddress = new Uri("https://localhost:7185/"); 

            var response = await httpClient.GetAsync("api/Blogs/GetLatestBlog"); 
            if (response.IsSuccessStatusCode)
            {
                var latestBlogs = await response.Content.ReadFromJsonAsync<List<Blog>>(); 
                return View(latestBlogs);
            }
            else
            {
                
                return View(new List<Blog>()); 
            }
        }
    }
}