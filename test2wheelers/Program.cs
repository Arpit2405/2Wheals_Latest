using Microsoft.AspNetCore.Authentication.Cookies;
using test2wheelers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SqlHelper>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";   // Login page
        options.LogoutPath = "/Login/Index"; // Logout page
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20); // 20 min expiry
        options.SlidingExpiration = true;
    });


var app = builder.Build(); 
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
