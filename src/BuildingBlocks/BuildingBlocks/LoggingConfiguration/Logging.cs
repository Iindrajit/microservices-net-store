using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
//using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace BuildingBlocks.LoggingConfiguration
{
    public static class Logging
    {
        public static Action<HostBuilderContext, LoggerConfiguration> ConfigureLogger =>
           (hostingContext, loggerConfiguration) =>
           {
               var env = hostingContext.HostingEnvironment;

               loggerConfiguration.MinimumLevel.Debug()
                   .Enrich.FromLogContext()
                   .Enrich.WithProperty("ApplicationName", env.ApplicationName)
                   .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
                   .Enrich.WithExceptionDetails()
                   .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                   .MinimumLevel.Override("System.Net.Http.HttpClient", LogEventLevel.Warning)
                   .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                   .WriteTo.Console();

               var elasticUri = hostingContext.Configuration.GetValue<string>("ElasticConfiguration:Url");
               if (!string.IsNullOrEmpty(elasticUri))
               {
                   loggerConfiguration.WriteTo.Elasticsearch(
                       new ElasticsearchSinkOptions(new Uri(elasticUri))
                       {
                           AutoRegisterTemplate = true,
                           AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                           IndexFormat = $"shopit-logs-{DateTime.UtcNow:yyyy.MM.dd}",
                           MinimumLogEventLevel = LogEventLevel.Debug
                       });
               }
           };
    }
}
