using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class TokenExchangeExtensionGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "urn:ietf:params:oauth:grant-type:token-exchange";

        private string _accessTokenType => "urn:ietf:params:oauth:token-type:access_token";

        private readonly ITokenValidator _tokenValidator;

        public TokenExchangeExtensionGrantValidator(ITokenValidator tokenValidator)
        {
            _tokenValidator = tokenValidator ?? throw new System.ArgumentNullException(nameof(tokenValidator));
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var requestedGrantType = context.Request.Raw.Get("grant_type");
            if (string.IsNullOrWhiteSpace(requestedGrantType) || requestedGrantType != GrantType)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid Grant.");
                return;
            }

            var subjectToken = context.Request.Raw.Get("subject_token");
            if (string.IsNullOrWhiteSpace(subjectToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject token missing.");
                return;
            }

            var subjectTokenType = context.Request.Raw.Get("subject_token_type");
            if (string.IsNullOrWhiteSpace(subjectTokenType))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject token type missing.");
                return;
            }

            if (subjectTokenType != _accessTokenType)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject token type is invalid.");
                return;
            }

            var result = await _tokenValidator.ValidateAccessTokenAsync(subjectToken);
            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject token is invalid.");
                return;
            }

            var subjectClaim = result.Claims.FirstOrDefault(c => c.Type == "sub");
            if (subjectClaim == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Subject token must contain sub value.");
                return;
            }

            context.Result = new GrantValidationResult(
                subjectClaim.Value,
                "access_token",
                result.Claims);

            return;
        }
    }
}