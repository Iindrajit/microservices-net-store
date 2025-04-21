using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Refit;
using Shopping.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddRazorPages();

var requireAuthenticatedUserPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
// Add services to the container.

builder.Services
    .AddRazorPages()
    .AddMvcOptions(options =>
        options.Filters.Add(new AuthorizeFilter(requireAuthenticatedUserPolicy))
        );

builder.Services.AddAccessTokenManagement();

//builder.Services.AddScoped<ICatalogService, CatalogService>();

//builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
//    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:AppGateway"]!))
//    .AddUserAccessTokenHandler();

//Refit
builder.Services.AddRefitClient<ICatalogService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:AppGateway"]!);
    })
    .AddUserAccessTokenHandler();


builder.Services.AddRefitClient<IBasketService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:AppGateway"]!);
    })
    .AddUserAccessTokenHandler();

builder.Services.AddRefitClient<IOrderingService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:AppGateway"]!);
    })
    .AddUserAccessTokenHandler();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = builder.Configuration["ApiSettings:IdentityServer"];
    options.ClientId = "ShoppingWebApp";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.ClientSecret = "Yy1VtCMf0MMK2BG9haWY/e63EOTcyA+iwBg0unfxH6o=";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.Scope.Add("custId");
    options.Scope.Add("appgateway.fullaccess");
    options.Scope.Add("offline_access");
    options.ClaimActions.MapJsonKey("custId", "custId");
    options.SignedOutCallbackPath = "/signout-callback-oidc";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
