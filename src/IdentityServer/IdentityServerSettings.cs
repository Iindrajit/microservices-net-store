using System.Collections.Generic;

namespace IdentityServer
{
    public class ClientDetail
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public List<string> ClientSecrets { get; set; }
        public List<string> AllowedGrantTypes { get; set; }
        public List<string> RedirectUris { get; set; }
        public string FrontChannelLogoutUri { get; set; } = string.Empty;
        public List<string> PostLogoutRedirectUris { get; set; }
        public bool AllowOfflineAccess { get; set; } = true;
        public List<string> AllowedScopes { get; set; }
        public bool RequireConsent { get; set; }
    }
}
