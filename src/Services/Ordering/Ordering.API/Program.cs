using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using System.Net;
using Ordering.API;
using Ordering.Application;
using Ordering.Infrastructure;
using Ordering.Infrastructure.Data.Extensions;
using BuildingBlocks.LoggingConfiguration;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog(Logging.ConfigureLogger);

//Add services
builder.Services.AddApplicationServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .AddApiServices(builder.Configuration);

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer"];
        options.Audience = "orderingapi";
    });

// Custom certificate logic
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

app.UseApiServices();

if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}

app.Run();

// Ensure Serilog is flushed on application shutdown
// (to ensure that all log entries are written before the application exits.)
Log.CloseAndFlush();