using Microsoft.EntityFrameworkCore;
using Pizzashop.Web;
using Pizzashop.Web.Extensions;
using Pizzashop.Web.Middlewares;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("pizzashopDbConnection");

DependencyInjection.RegisterServices(builder.Services, connectionString!);

// Add JWT Bearer authentication
builder.AddAuthenticationAndAuthorization();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error");

app.UseRotativa();


app.UseSession(); //Enable Session

// Middleware for refresh accesstoken if refreshtoken available
app.UseCreateAccessToken();

// Middleware to set user ID in session
app.UseSetSessionData();

app.UseFirstLogin();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseGlobalExceptionHandler();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
