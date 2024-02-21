using Microsoft.AspNetCore.Mvc;
using PROJE_UI.Models;

namespace PROJE_UI.ViewComponents
{
    public class EditProfile : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EditProfile(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Request.Cookies["UserId"];

            var httpClient = _httpClientFactory.CreateClient();

            httpClient.BaseAddress = new Uri("https://localhost:7185/");

            var response = await httpClient.GetAsync($"/api/Users/GetById?UserId={userId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<User>();
                return View(result);
            }
            else
            {

                return View(new User());
            }




        }
    }
}
