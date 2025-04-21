using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ShoppingGateway.Handlers;
using ShoppingGateway.Policies;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", false, true);

const string authenticationScheme = "AppGatewayAuthenticationScheme";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(authenticationScheme, options =>
    {
        options.Authority = builder.Configuration["IdentityServer"];
        options.Audience = "appgateway";
    });

builder.Services.AddSingleton<IPolicyHolder, PolicyHolder>();

builder.Services.AddHttpClient("IdentityService", config =>
{
    config.BaseAddress = new Uri(builder.Configuration["IdentityServer"]);
});

builder.Services.AddAccessTokenManagement();

builder.Services.AddScoped<TokenExchangeDelegatingHandler>();

builder.Services.AddOcelot()
    .AddDelegatingHandler<TokenExchangeDelegatingHandler>();
    

// Configure Kestrel for HTTPS with your custom certificate
builder.WebHost.ConfigureKestrel((ctx, options) =>
{
    var useCustomLocalCert = ctx.Configuration.GetValue<bool>("UseCustomLocalCert");
    if (useCustomLocalCert)
    {
        options.Listen(IPAddress.Any, 8081, listenOptions =>
        {
            listenOptions.UseHttps("keys/cr-id-local.pfx", "Test1");
        });
    }
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

await app.UseOcelot();

app.Run();
