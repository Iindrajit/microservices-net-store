using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Polly;
using ShoppingGateway.Policies;

namespace ShoppingGateway.Handlers
{
    public class TokenExchangeDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;
        private readonly IPolicyHolder _policyHolder;

        public TokenExchangeDelegatingHandler(
            IHttpClientFactory httpClientFactory, 
            IClientAccessTokenCache clientAccessTokenCache,
            IPolicyHolder policyHolder
            )
        {
            _httpClientFactory = httpClientFactory;
            _clientAccessTokenCache = clientAccessTokenCache;
            _policyHolder = policyHolder;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization == null || string.IsNullOrWhiteSpace(request.Headers.Authorization.Parameter))
            {
                throw new HttpRequestException("Access token is missing or invalid.");
            }
            // extract the token
            var incomingToken = request.Headers.Authorization.Parameter;

            // set the bearer token
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    await GetAccessToken(incomingToken));

            var combinedPolicy = Policy.WrapAsync(
                _policyHolder.HttpFallbackPolicy,
                _policyHolder.HttpCircuitBreakerPolicy,
                _policyHolder.HttpRetryPolicy,
                _policyHolder.HttpTimeoutPolicy
            );

            return await combinedPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
        }

        private async Task<string> GetAccessToken(string incomingToken)
        {
            var item = await _clientAccessTokenCache
                .GetAsync("gatewaytodownstreamtokenexchange_apis", null);
            if (item != null)
            {
                return item.AccessToken;
            }

            var (accessToken, expiresIn) = await ExchangeToken(incomingToken);

            await _clientAccessTokenCache.SetAsync(
                "gatewaytodownstreamtokenexchange_apis",
                accessToken,
                expiresIn,
                null);

            return accessToken;
        }

        private async Task<(string, int)> ExchangeToken(string incomingToken)
        {
            var client = _httpClientFactory.CreateClient("IdentityService");

            var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync();

            if (discoveryDocumentResponse.IsError)
            {
                throw new Exception(discoveryDocumentResponse.Error);
            }

            var customParams = new Parameters
            {
                { "subject_token_type", "urn:ietf:params:oauth:token-type:access_token"},
                { "subject_token", incomingToken},
                { "scope", "openid profile custId catalogapi.fullaccess basketapi.fullaccess orderingapi.fullaccess" }
            };

            var tokenResponse = await client.RequestTokenAsync(new TokenRequest()
            {
                Address = discoveryDocumentResponse.TokenEndpoint,
                GrantType = "urn:ietf:params:oauth:grant-type:token-exchange",
                Parameters = customParams,
                ClientId = "gatewaytodownstreamtokenexchange",
                ClientSecret = "ZgjRwbdBtF15gH2zIvL4gvAdleLwnZDLuV97v18+EKM="
            });

            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.Error);
            }

            return (tokenResponse.AccessToken, tokenResponse.ExpiresIn);
        }
    }
}
