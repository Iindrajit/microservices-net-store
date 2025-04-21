using Polly;

namespace ShoppingGateway.Policies
{
    public interface IPolicyHolder
    {
        IAsyncPolicy<HttpResponseMessage> HttpTimeoutPolicy { get; set; }
        IAsyncPolicy<HttpResponseMessage> HttpRetryPolicy { get; set; }
        IAsyncPolicy<HttpResponseMessage> HttpCircuitBreakerPolicy { get; set; }
        IAsyncPolicy<HttpResponseMessage> HttpFallbackPolicy { get; set; }
    }
}
