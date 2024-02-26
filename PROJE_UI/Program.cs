using Microsoft.AspNetCore.Authentication.Cookies;
using PROJE_UI;
var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ApiServiceOptions>(options =>
{
    options.BaseUrl = new Uri("https://localhost:7185/");
});

builder.Services.AddHttpClient();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
     .AddCookie(options =>
     {
         options.LogoutPath = "/User/Logout"; 
     });
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute(); 
app.Run();