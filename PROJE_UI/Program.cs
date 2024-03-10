using Microsoft.AspNetCore.Authentication.Cookies;
using PROJE_UI;
using PROJE_UI.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ApiServiceOptions>(options =>
{
    options.BaseUrl = new Uri("https://localhost:7185");
});
var apiServiceOptions = new ApiServiceOptions
{
    BaseUrl = new Uri("https://localhost:7185")
};
builder.Services.AddSingleton(apiServiceOptions);

   
builder.Services.AddSession();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     .AddCookie(options =>
     {
         options.LogoutPath = "/User/Logout";
         options.LogoutPath = "/Admin/Logout";
         options.ExpireTimeSpan = TimeSpan.FromDays(1);
     });
builder.Services.AddControllersWithViews();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.AddScoped<StripeSettings>();

var app = builder.Build();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapDefaultControllerRoute(); 
app.Run();
