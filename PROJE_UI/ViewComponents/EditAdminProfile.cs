using Microsoft.AspNetCore.Mvc;
using PROJE_UI.Models;

namespace PROJE_UI.ViewComponents
{
    public class EditAdminProfile:ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiServiceOptions _apiServiceOptions;
        public EditAdminProfile(IHttpClientFactory httpClientFactory, ApiServiceOptions apiServiceOptions)
        {
            _httpClientFactory = httpClientFactory;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var adminId = HttpContext.Request.Cookies["AdminId"];

            var httpClient = _httpClientFactory.CreateClient();



            var response = await httpClient.GetAsync($"{BaseUrl}api/Admins/GetById?AdminId={adminId}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Admin>();
                return View(result);
            }
            else
            {

                return View(new Admin());
            }




        }

    }
}
