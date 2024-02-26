using Application;
using Domain.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Persistence;
using Persistence.DataBase;
using WebApp;

var builder = WebApplication.CreateBuilder(args);

// Authenticated User Policies
var authenticatedUserPolicies = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();

// Add Session service 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Add layers
builder.Services
        .AddWebApp()
        .AddApplication()
        .AddPersistence(builder.Configuration);

// Add Authentication
builder.Services.AddIdentity<UserModel, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

}).AddEntityFrameworkStores<LoginAppContext>()
.AddDefaultTokenProviders();

// Setting login view and register view
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";
});

// Setting ClientId and ClientSecret
builder.Services.AddAuthentication()
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value ?? "";
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value ?? "";
});

// Add Authorization service
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter(authenticatedUserPolicies));
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

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
