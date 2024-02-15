using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;

namespace PROJE_UI.ViewComponents
{
	public class AnnouncementDetails:ViewComponent
	{
		private readonly IHttpClientFactory _httpClientFactory;

		public AnnouncementDetails(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var httpClient = _httpClientFactory.CreateClient();

			httpClient.BaseAddress = new Uri("https://localhost:7185/");

			var response = await httpClient.GetAsync("/api/Announcements/GetLatestAnnouncement");
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
