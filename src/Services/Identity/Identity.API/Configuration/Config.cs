using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lontray.Services.Identity.API.Configuration
{
    public class Config
    {

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>() {
                new ApiResource("WebApi", "Web API"),
                new ApiResource("webbffshopping","Web Bff for Shopping") // ResourceName corresponds to Apis' audience
                {
                    //Here we are creating a relation between ApiResources and ApiScopes
                    Scopes = new List<string> { "webbffshopping.all" }
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>() {
                new ApiScope("WebApi.manage", "Can manage Web API"),
                new ApiScope("WebApi.read", "Can query Web API"),
                new ApiScope("webbffshopping.all", "Can access to Web Bff for Shopping"), // Corresponds to Clients' allowed scopes
            };


        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "Tenant",
                    UserClaims = new List<string> {"tenantid"}
                }
            };

        public static IEnumerable<Client> Clients(Dictionary<string, string> clientsUrl)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "webbffshoppingswaggerui",
                    ClientName = "web shopping bff Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["WebBffShopping"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["WebBffShopping"]}/swagger/" },

                    AllowedScopes =
                    {
                        "webbffshopping.all",
                    }
                },
                new Client
                {
                    ClientId = "WebMVC",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "WebApi.manage" }
                },
                new Client
                {
                    ClientId = "WebMVCIdentity",

                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = new List<string> {"https://localhost:7001/signin-oidc"},
                    PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "tenant"
                    },
                    RequirePkce = true,
                    AllowPlainTextPkce = false,

                    // Required if you always want User claims to be included within IdentityToken
                    AlwaysIncludeUserClaimsInIdToken = true,
                },
                new Client
                {
                    ClientId = "WebApi",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "WebApi.manage" }
                },

            };
        }

    }
}
