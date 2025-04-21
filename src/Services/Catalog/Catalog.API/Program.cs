using BuildingBlocks.LoggingConfiguration;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog(Logging.ConfigureLogger);

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddCarter();
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();

if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["IdentityServer"];
        options.Audience = "catalogapi";
    });

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

app.UseExceptionHandler(options => { });

app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.MapCarter();

app.Run();