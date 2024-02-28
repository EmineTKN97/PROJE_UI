﻿using Microsoft.AspNetCore.Mvc;
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
        private readonly ApiServiceOptions _apiServiceOptions;

        public BiletController(HttpClient client, ApiServiceOptions apiServiceOptions)
        {
            _client = client;
            _apiServiceOptions = apiServiceOptions;
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpGet]
        public async Task<IActionResult> BiletSatis()
        {
            var citiesResponse = await _client.GetAsync($"{BaseUrl}api/Cities/GetAllCities");

            if (citiesResponse.IsSuccessStatusCode)
            {
                var citiesApiResponse = await citiesResponse.Content.ReadAsStringAsync();
                var citiesResult = JsonConvert.DeserializeObject<ApiResponseModel<List<City>>>(citiesApiResponse);

                if (citiesResult.Success)
                {
                    var districtsResponse = await _client.GetAsync($"{BaseUrl}api/Districts/GetAllDistricts");

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
        public async Task<IActionResult> MuseumDetails(string city,string district)
            {
                string apiKey = "93k9I6pzPzDC3Nnc4tsJLdK86pCLIUySdbmpxfHqTmnsQyWdQ1tipEf0AnQ7";
                var museumResponse = await _client.GetAsync($" https://www.nosyapi.com/apiv2/service/museum?city={city}&district={district}&apiKey={apiKey}");

            if (museumResponse.IsSuccessStatusCode)
                {
              

                var museumApiResponse = await museumResponse.Content.ReadAsStringAsync();
                    var museumResult = JsonConvert.DeserializeObject<MuseumApiResponse>(museumApiResponse);

                    if (museumResult.status == "success")
                    {
                        
                        return View("MuseumDetails",  museumResult.data); 
                    }
                    else
                    {
                      
                        return RedirectToAction("Error", new { message = museumResult.message });
                    }
                }
                else
                {
                    
                    return RedirectToAction("Error", new { message = "Müze API ile iletişim sırasında bir hata oluştu." });
                }
            }



        }
}