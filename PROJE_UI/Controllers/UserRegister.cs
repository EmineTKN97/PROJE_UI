using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using NuGet.Common;
using PROJE_UI.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;

namespace PROJE_UI.Controllers
{
	public class UserRegister : Controller
	{
        private readonly HttpClient _client;

        public UserRegister(HttpClient client)
        {
            _client = client;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new User());
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            using (var response = await _client.PostAsync("https://localhost:7185/api/Auth/registerUser", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    var addUser = JsonConvert.DeserializeObject<User>(apiResponse);
                    return RedirectToAction("Login");
                }
                else
                { 
                    ModelState.AddModelError("", "Kullanıcı kaydedilemedi. Lütfen tekrar deneyin.");
                    return View(model);
                }
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
             StringContent content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var handler = new JwtSecurityTokenHandler();
            using (var response = await _client.PostAsync("https://localhost:7185/api/Auth/loginUser", content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        var data = JsonConvert.DeserializeObject<TokenOptions>(apiResponse);
                        var token = handler.ReadJwtToken(data.Token);
                        return RedirectToAction("Blog", "Blog");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Giriş Yapılamadı. Lütfen tekrar deneyin.");
                        return View(model);
                    }
                }
            }
           
        }
    }

