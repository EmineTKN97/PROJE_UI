using Microsoft.AspNetCore.Mvc;
using PROJE_UI.Models;

namespace PROJE_UI.ViewComponents
{
    public class EditProfile : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiServiceOptions _apiServiceOptions;
        public EditProfile(IHttpClientFactory httpClientFactory, ApiServiceOptions apiServiceOptions)
        {
            _httpClientFactory = httpClientFactory;
            _apiServiceOptions = apiServiceOptions; 
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.Request.Cookies["UserId"];

            var httpClient = _httpClientFactory.CreateClient();



            var response = await httpClient.GetAsync($"{BaseUrl}api/Users/GetById?UserId={userId}");
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
