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
        public PaymentController(StripeSettings stripeSettings, ApiServiceOptions apiServiceOptions, HttpClient client)
        {
            _stripeSettings = stripeSettings;
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
        public IActionResult Payment(string Price)
        {
           
                string currency = "usd";
                string successUrl = "http://localhost:5131/Home/Success";
                string cancelUrl = "http://localhost:5131/Home/Cancel";

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
                            
                        },
                        Quantity = 1
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
