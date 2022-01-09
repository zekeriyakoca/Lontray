using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Lontray.Services.Identity.API.Configuration
{
    public class Config
    {

        public static IEnumerable<ApiResource> ApiResources =>
            new List<ApiResource>() {
                // ResourceName corresponds to Apis' audience
                new ApiResource("webbffshopping","Web Bff for Shopping")
                {
                    //Here we are creating a relation between ApiResources and ApiScopes
                    Scopes = new List<string> { "webbffshopping.all" }
                },
                new ApiResource("basketApi", "Basket API")
                {
                    Scopes = new List<string> { "basketApi.read", "basketApi.all" }
                },
                new ApiResource("orderApi", "Order API")
                {
                    Scopes = new List<string> { "orderApi.read", "orderApi.all" }
                },
                new ApiResource("catalogApi", "Catalog API")
                {
                    Scopes = new List<string> { "catalogApi.read", "catalogApi.all" }
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>() {
                new ApiScope("webbffshopping.all", "Can access to Web Bff for Shopping"), // Corresponds to Clients' allowed scopes
                
                new ApiScope("basketApi.read", "Can query Basket API"),
                new ApiScope("basketApi.all", "Can manage Basket API"),
                new ApiScope("orderApi.read", "Can query Order API"),
                new ApiScope("orderApi.all", "Can manage Order API"),
                new ApiScope("catalogApi.read", "Can query Catalog API"),
                new ApiScope("catalogApi.all", "Can manage Catalog API"),
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
                    AllowedScopes = { "webApi.manage", "basketApi.all", "orderApi.all", "catalogApi.all" }
                },
                new Client
                {
                    ClientId = "WebMVCIdentity",

                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = new List<string> {$"{clientsUrl["WebMVCIdentity"]}/signin-oidc"},
                    PostLogoutRedirectUris = { $"{clientsUrl["WebMVCIdentity"]}/signout-callback-oidc" },
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
                    AllowedScopes = { "webApi.manage" }
                },
                new Client
                {
                    ClientId = "basketswaggerui",
                    ClientName = "Basket API Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["BasketApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["BasketApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "basketApi.all",
                    }
                },
                new Client
                {
                    ClientId = "catalogswaggerui",
                    ClientName = "Catalog API Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["CatalogApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["CatalogApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "catalogApi.all",
                    }
                },
                new Client
                {
                    ClientId = "orderingswaggerui",
                    ClientName = "Ordering API Swagger UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris = { $"{clientsUrl["OrderingApi"]}/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { $"{clientsUrl["OrderingApi"]}/swagger/" },

                    AllowedScopes =
                    {
                        "orderingApi.all",
                    }
                },

            };
        }

    }
}
