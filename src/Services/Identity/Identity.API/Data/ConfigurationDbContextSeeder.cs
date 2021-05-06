using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Lontray.Services.Identity.API.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lontray.Services.Identity.API.Data
{
    public class ConfigurationDbContextSeeder
    {
        public async Task SeedAsync(ConfigurationDbContext context, IConfiguration configuration)
        {

            ////callbacks urls from config:
            //var clientUrls = new Dictionary<string, string>();

            //clientUrls.Add("Mvc", configuration.GetValue<string>("MvcClient"));
            //clientUrls.Add("Spa", configuration.GetValue<string>("SpaClient"));
            //clientUrls.Add("Xamarin", configuration.GetValue<string>("XamarinCallback"));
            //clientUrls.Add("BasketApi", configuration.GetValue<string>("BasketApiClient"));
            //clientUrls.Add("OrderingApi", configuration.GetValue<string>("OrderingApiClient"));
            //clientUrls.Add("MobileShoppingAgg", configuration.GetValue<string>("MobileShoppingAggClient"));
            //clientUrls.Add("WebShoppingAgg", configuration.GetValue<string>("WebShoppingAggClient"));
            //clientUrls.Add("WebhooksApi", configuration.GetValue<string>("WebhooksApiClient"));
            //clientUrls.Add("WebhooksWeb", configuration.GetValue<string>("WebhooksWebClient"));

            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var api in Config.ApiResources)
                {
                    context.ApiResources.Add(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                await context.SaveChangesAsync();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var api in Config.ApiScopes)
                {
                    context.ApiScopes.Add(api.ToEntity());
                }

                await context.SaveChangesAsync();
            }


            if (!context.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    context.ApiResources.Add(resource.ToEntity());
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
