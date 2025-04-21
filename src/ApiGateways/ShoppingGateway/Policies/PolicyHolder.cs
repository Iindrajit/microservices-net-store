using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using System.Net;

namespace ShoppingGateway.Policies
{
    public class PolicyHolder : IPolicyHolder
    {
        public IAsyncPolicy<HttpResponseMessage> HttpTimeoutPolicy { get; set; }
        public IAsyncPolicy<HttpResponseMessage> HttpRetryPolicy { get; set; }
        public IAsyncPolicy<HttpResponseMessage> HttpCircuitBreakerPolicy { get; set; }
        public IAsyncPolicy<HttpResponseMessage> HttpFallbackPolicy { get; set; }

        public PolicyHolder(IConfiguration configuration, ILogger<PolicyHolder> logger)
        {
            HttpTimeoutPolicy = GetTimeoutPolicy(logger);
            HttpRetryPolicy = GetRetryPolicy(configuration, logger);
            HttpCircuitBreakerPolicy = GetCircuitBreakerPolicy(configuration, logger);
            HttpFallbackPolicy = GetFallbackPolicy(logger);
        }

        private IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(ILogger<PolicyHolder> logger)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                                    TimeSpan.FromSeconds(5), // Timeout after 5 seconds
                                    TimeoutStrategy.Optimistic, // Cancels running operations when timeout occurs
            onTimeoutAsync: (context, timespan, task) =>
            {
                logger.LogWarning("Request timed out after {Timeout}s.", timespan.TotalSeconds);
                return Task.CompletedTask;
            });
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IConfiguration configuration, ILogger<PolicyHolder> logger)
        {
            var retryCount = configuration.GetValue("PolicySettings:RetryCount", 3);

            return Policy.Handle<HttpRequestException>()  // Handles exceptions like HTTP request errors
                .Or<Refit.ApiException>()                 // Handles Refit exceptions
                .OrResult<HttpResponseMessage>(r => r.StatusCode != HttpStatusCode.NotFound && !r.IsSuccessStatusCode) // Handles non-successful HTTP responses
                .WaitAndRetryAsync(
                    retryCount,
                    retryAttempt => TimeSpan.FromMilliseconds(1), //(Math.Pow(2, retryAttempt)), // Exponential backoff
                    onRetryAsync: (outcome, timespan, retryAttempt, context) =>
                    {
                        // Log the retry attempt
                        logger.LogWarning("Retry attempt {RetryAttempt} after {Delay} due to {Reason}",
                            retryAttempt,
                            timespan,
                            outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                        return Task.CompletedTask;
                    }
                );
        }

        private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(
            IConfiguration configuration,
            ILogger<PolicyHolder> logger)
        {
            var failureThreshold = configuration.GetValue("PolicySettings:FailureThreshold", 3);
            var durationOfBreak = configuration.GetValue("PolicySettings:DurationOfBreakInSeconds", 10);

            return Policy.Handle<HttpRequestException>()  // Handles HTTP request exceptions
                .Or<Refit.ApiException>()
                .OrResult<HttpResponseMessage>(r => r.StatusCode != HttpStatusCode.NotFound && !r.IsSuccessStatusCode) // Handles non-successful HTTP responses
                .CircuitBreakerAsync(
                    failureThreshold,
                    TimeSpan.FromSeconds(durationOfBreak), // Circuit stays open for configured seconds
                     onBreak: (outcome, breakDelay) =>
                     {
                         logger.LogWarning("Circuit breaker opened due to {Reason}. Remaining open for {BreakDelay}.",
                             outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString(),
                             breakDelay);
                     },
                    onReset: () =>
                    {
                        logger.LogInformation("Circuit breaker reset. Requests will be allowed again.");
                    },
                    onHalfOpen: () =>
                    {
                        logger.LogInformation("Circuit breaker is half-open. Testing downstream availability.");
                    }
                );
        }

        private IAsyncPolicy<HttpResponseMessage> GetFallbackPolicy(ILogger<PolicyHolder> logger)
        {
            return Policy< HttpResponseMessage>
                .Handle<HttpRequestException>() // Handle exceptions
                .Or<Refit.ApiException>()
                .Or<BrokenCircuitException>()   // Handle circuit breaker exceptions
                .OrResult(r => !r.IsSuccessStatusCode) // Handle unsuccessful HTTP responses
                .FallbackAsync(
                    fallbackAction: (ct) =>
                    {
                        // Return a custom fallback response
                        var fallbackResponse = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            //Content = new StringContent("Oops, something went wrong. Please try again later.")

                            Content = new StringContent(
                                "{\"message\": \"Oops, something went wrong. Please try again later.\"}", 
                            System.Text.Encoding.UTF8, 
                            "application/json")
                        };
                        return Task.FromResult(fallbackResponse);
                    },
                    onFallbackAsync: (outcome) =>
                    {
                        // Log the fallback execution
                        logger.LogWarning("Fallback executed due to: {Reason}",
                            outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                        return Task.CompletedTask;
                    });
        }

    }
}
