using Microsoft.AspNetCore.Mvc;
using PROJE_UI.Models;

namespace PROJE_UI.ViewComponents
{
    public class Slidebar : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiServiceOptions _apiServiceOptions;
        public Slidebar(IHttpClientFactory httpClientFactory, ApiServiceOptions apiServiceOptions)
        {
            _httpClientFactory = httpClientFactory;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var httpClient = _httpClientFactory.CreateClient(); 

       

            var response = await httpClient.GetAsync($"{BaseUrl}api/Blogs/GetLatestBlog"); 
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