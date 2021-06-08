using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Lontray.Services.Identity.API.Configuration;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lontray.Services.Identity.API.Data
{
    public class ConfigurationDbContextSeeder
    {
        public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
        {

            //callbacks urls from config:
            var clientUrls = new Dictionary<string, string>();

            clientUrls.Add("WebBffShopping", configuration.GetValue<string>("Urls:WebBffShoppingClient"));
            clientUrls.Add("BasketApi", configuration.GetValue<string>("Urls:BasketApi"));
            clientUrls.Add("OrderingApi", configuration.GetValue<string>("Urls:OrderingApi"));
            clientUrls.Add("CatalogApi", configuration.GetValue<string>("Urls:CatalogApi"));

            if (bool.Parse(configuration["RecreateIdentityTables"]))
            {
                context.ApiResources.RemoveRange(context.ApiResources.ToList());
                context.ApiScopes.RemoveRange(context.ApiScopes.ToList());
                context.IdentityResources.RemoveRange(context.IdentityResources.ToList());
                context.Clients.RemoveRange(context.Clients.ToList());

                await context.SaveChangesAsync();
            }


            if (!context.ApiResources.Any())
            {
                foreach (var api in Config.ApiResources)
                {
                    context.ApiResources.Add(api.ToEntity());
                }
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var api in Config.ApiScopes)
                {
                    context.ApiScopes.Add(api.ToEntity());
                }

            }

            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients(clientUrls))
                {
                    context.Clients.Add(client.ToEntity());
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
