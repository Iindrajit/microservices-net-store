// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4;

namespace IdentityServerHost.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "123 St",
                    locality = "Victoria",
                    postal_code = 3000,
                    country = "Australia"
                };
                
                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "818727",
                        Username = "john",
                        Password = "john",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "John Doe"),
                            new Claim(JwtClaimTypes.GivenName, "John"),
                            new Claim(JwtClaimTypes.FamilyName, "Doe"),
                            new Claim(JwtClaimTypes.Email, "john@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://john.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("custId", "189DC8DC-990F-48E0-A37B-E6F2B60B9D7D")
                        }
                    },
                    new TestUser
                    {
                        SubjectId = "88421113",
                        Username = "jane",
                        Password = "jane",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Jane Doe"),
                            new Claim(JwtClaimTypes.GivenName, "Jane"),
                            new Claim(JwtClaimTypes.FamilyName, "Doe"),
                            new Claim(JwtClaimTypes.Email, "jane@email.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://jane.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("custId", "58C49479-EC65-4DE2-86E7-033C546291AA")
                        }
                    }
                };
            }
        }
    }
}