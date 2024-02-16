using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PROJE_UI.Models;
using PROJE_UI.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PROJE_UI.Controllers
{
    public class BiletController : Controller
    {
        private readonly HttpClient _client;


        public BiletController(HttpClient client)
        {
            _client = client;
        }

        [HttpGet]
        public async Task<IActionResult> BiletSatis()
        {
            var citiesResponse = await _client.GetAsync("https://localhost:7185/api/Cities/GetAllCities");

            if (citiesResponse.IsSuccessStatusCode)
            {
                var citiesApiResponse = await citiesResponse.Content.ReadAsStringAsync();
                var citiesResult = JsonConvert.DeserializeObject<ApiResponseModel<List<City>>>(citiesApiResponse);

                if (citiesResult.Success)
                {
                    var districtsResponse = await _client.GetAsync("https://localhost:7185/api/Districts/GetAllDistricts");

                    if (districtsResponse.IsSuccessStatusCode)
                    {
                        var districtsApiResponse = await districtsResponse.Content.ReadAsStringAsync();
                        var districtsResult = JsonConvert.DeserializeObject<ApiResponseModel<List<District>>>(districtsApiResponse);

                        if (districtsResult.Success)
                        {
                            var model = new BiletSatisViewModel
                            {
                                Cities = citiesResult.Data,
                                Districts = districtsResult.Data,
                            };

                            return View(model);
                        }
                        else
                        {
                            return RedirectToAction("Error", new { message = districtsResult.Message });
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error", new { message = "İlçeler API ile iletişim sırasında bir hata oluştu." });
                    }
                }
                else
                {
                    return RedirectToAction("Error", new { message = citiesResult.Message });
                }
            }
            else
            {
                return RedirectToAction("Error", new { message = "Şehirler API ile iletişim sırasında bir hata oluştu." });
            }
        }
            [HttpGet]
            public async Task<IActionResult> MuseumDetails()
            {
        
        return View();
        
            }
        


     
    }
}