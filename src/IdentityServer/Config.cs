// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServer
{
    public class Config
    {
        private readonly IConfiguration _configuration;

        public Config(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public static IEnumerable<IdentityResource> IdentityResources =>
           new IdentityResource[]
           {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("custId", new List<string>{ "custId"})
           };


        public static IEnumerable<ApiResource> ApiResources =>
           new ApiResource[]
           {
                new ApiResource("appgateway", "App Gateway")
                {
                    Scopes = { "appgateway.fullaccess" }
                },
                new ApiResource("catalogapi", "Catalog API")
                {
                    Scopes = { "catalogapi.fullaccess" }
                },
                new ApiResource("basketapi", "Basket API")
                {
                    Scopes = { "basketapi.fullaccess" }
                },
                new ApiResource("orderingapi", "Ordering API")
                {
                    Scopes = { "orderingapi.fullaccess" }
                }
           };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("appgateway.fullaccess"),
                new ApiScope("catalogapi.fullaccess"),
                new ApiScope("basketapi.fullaccess"),
                new ApiScope("orderingapi.fullaccess"),
            };

        public IEnumerable<Client> GetClients()
        {
            try
            {
                var clientSettings = _configuration.GetSection("IdentityServer:Clients").Get<List<ClientDetail>>();

                if (clientSettings == null)
                {
                    return Enumerable.Empty<Client>();
                }

                var clients = clientSettings.Select(clientSetting => new Client
                {
                    ClientId = clientSetting.ClientId,
                    ClientName = clientSetting.ClientName,
                    ClientSecrets = clientSetting.ClientSecrets
                            .Select(s => new Secret(s.Sha256()))
                            .ToHashSet(),
                    AllowedGrantTypes = clientSetting.AllowedGrantTypes,
                    AllowedScopes = clientSetting.AllowedScopes,
                    AllowOfflineAccess = clientSetting.AllowOfflineAccess,
                    RequireConsent = clientSetting.RequireConsent,
                    FrontChannelLogoutUri = Environment.GetEnvironmentVariable("FrontChannelLogoutUri")
                    ?? clientSetting.FrontChannelLogoutUri ?? string.Empty,

                    PostLogoutRedirectUris = Environment.GetEnvironmentVariable("PostLogoutRedirectUris") != null
                    ? new List<string> { Environment.GetEnvironmentVariable("PostLogoutRedirectUris") }
                    : clientSetting.PostLogoutRedirectUris ?? new List<string>(),

                    RedirectUris = Environment.GetEnvironmentVariable("RedirectUris") != null
                    ? new List<string> { Environment.GetEnvironmentVariable("RedirectUris") }
                    : clientSetting.RedirectUris ?? new List<string>(),
                });

                return clients;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}