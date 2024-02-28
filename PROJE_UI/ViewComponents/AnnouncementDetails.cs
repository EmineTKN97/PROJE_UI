using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;

namespace PROJE_UI.ViewComponents
{
	public class AnnouncementDetails:ViewComponent
	{
		private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiServiceOptions _apiServiceOptions;
        public AnnouncementDetails(IHttpClientFactory httpClientFactory, ApiServiceOptions apiServiceOptions)
		{
			_httpClientFactory = httpClientFactory;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var httpClient = _httpClientFactory.CreateClient();

			var response = await httpClient.GetAsync($"{BaseUrl}api/Announcements/GetLatestAnnouncement");
			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<List<Announcement>>();
				return View(result); 
			}
			else
			{

				return View(new List<Announcement>());
			}


		}
	}
}
