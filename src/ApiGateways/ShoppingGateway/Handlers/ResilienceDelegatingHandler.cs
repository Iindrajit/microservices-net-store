using Polly;
using ShoppingGateway.Policies;

namespace ShoppingGateway.Handlers
{
    public class ResilienceDelegatingHandler : DelegatingHandler
    {
        private readonly IPolicyHolder _policyHolder;
        public ResilienceDelegatingHandler(IPolicyHolder policyHolder) 
        {
            _policyHolder = policyHolder;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var combinedPolicy = Policy.WrapAsync(
            _policyHolder.HttpFallbackPolicy,
            _policyHolder.HttpRetryPolicy,
            _policyHolder.HttpCircuitBreakerPolicy
            );

            return await combinedPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }
    }
}
