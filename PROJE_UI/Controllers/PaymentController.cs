using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PROJE_UI.Models;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Net.Http.Headers;


namespace PROJE_UI.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _client;
        private readonly StripeSettings _stripeSettings;
        private readonly ApiServiceOptions _apiServiceOptions;
        public string SessionId { get; set; }
        public PaymentController(IOptions<StripeSettings> stripeSettings, ApiServiceOptions apiServiceOptions, HttpClient client)
        {
            _stripeSettings = stripeSettings.Value;
            _apiServiceOptions = apiServiceOptions;
            _client = client;   
        }
        private Uri BaseUrl => _apiServiceOptions.BaseUrl;
        [HttpGet]
        public async Task<IActionResult> Payment()
        {
            var userId = HttpContext.Request.Cookies["UserId"];
            var bearerToken = HttpContext.Request.Cookies["Bearer"];
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            var response = await _client.GetAsync($"{BaseUrl}api/Tickets/GetByUserId?UserId={userId}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Ticket>(apiResponse);
                    return View(result);
            }
            var errorResponse = await response.Content.ReadAsStringAsync();
            TempData["ErrorListTicket"] = errorResponse;
            return RedirectToAction("BiletSatis", "Bilet");
        }
        [HttpPost]
        public IActionResult Payment(decimal Price, string MuseumName, int Quantity,string UserName,string UserSurName)
        {

            string currency = "usd";
            string successUrl = "https://localhost:7019/UserEdit/Success";
            string cancelUrl = "https://localhost:7019/UserEdit/Error";

            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

            SessionCreateOptions options = new()
            {
                PaymentMethodTypes = new List<string>()
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions> {
                    new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            Currency = currency,
                            UnitAmount = Convert.ToInt32(Price) * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {

                                Name =$"{UserName} {UserSurName}",
                                Description= MuseumName
                               
                            }

                            },
                            Quantity =1,
                        },

                    },
                    Mode = "payment",
                    SuccessUrl = successUrl,
                    CancelUrl = cancelUrl
                };


            SessionService sessionService = new();

            Session session = sessionService.Create(options);
            SessionId = session.Id;

            return Redirect(session.Url);
        
            


        }
    }
}
