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
                new ApiResource("BasketApi", "Basket API")
                {
                    Scopes = new List<string> { "BasketApi.read", "BasketApi.all" }
                },
                new ApiResource("OrderApi", "Order API")
                {
                    Scopes = new List<string> { "OrderApi.read", "OrderApi.all" }
                },
                new ApiResource("CatalogApi", "Catalog API")
                {
                    Scopes = new List<string> { "CatalogApi.read", "CatalogApi.all" }
                },
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>() {
                new ApiScope("webbffshopping.all", "Can access to Web Bff for Shopping"), // Corresponds to Clients' allowed scopes
                
                new ApiScope("BasketApi.read", "Can query Basket API"),
                new ApiScope("BasketApi.all", "Can manage Basket API"),
                new ApiScope("OrderApi.read", "Can query Order API"),
                new ApiScope("OrderApi.all", "Can manage Order API"),
                new ApiScope("CatalogApi.read", "Can query Catalog API"),
                new ApiScope("CatalogApi.all", "Can manage Catalog API"),
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
                    AllowedScopes = { "WebApi.manage", "BasketApi.all", "OrderApi.all", "CatalogApi.all" }
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
                        "basket.all",
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
                        "catalog.all",
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
                        "ordering.all",
                    }
                },

            };
        }

    }
}
