using Catalog.API.Infrastructure;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogGrpc
{
    public class CatalogService : Catalog.CatalogBase
    {
        private readonly ILogger<CatalogService> logger;
        private readonly CatalogContext dbContext;

        public CatalogService(ILogger<CatalogService> logger, CatalogContext context)
        {
            this.logger = logger;
            this.dbContext = context;
        }
        // TODO : Test 
        public override Task<CheckAvailibilityResponse> ValidateBasket(CheckAvailibilityRequest request, ServerCallContext context)
        {
            var ids = request?.Items?.Select(i => i.Id).ToList();
            if (ids == null || ids.Count == 0)
            {
                throw new ArgumentException("Unable to validate basket. No item id found");
            }
            var catalogItems = dbContext.CatalogItems
                .Where(i => ids.Contains(i.Id))
                .Select(i =>
                new
                {
                    Id = i.Id,
                    AvailableStock = i.AvailableStock
                }).ToList();

            var itemAvailibilityResponses = request.Items.Select(i =>
            {
                var itemInCatalog = catalogItems?.Where(item => item.Id == i.Id).SingleOrDefault();
                var availiblity = itemInCatalog == null ? -1 : itemInCatalog.AvailableStock;
                return new CatalogItemAvailibilityResponse
                {
                    Availibility = availiblity >= i.Quantity,
                    Id = i.Id
                };
            });

            var response = new CheckAvailibilityResponse();
            response.Items.AddRange(itemAvailibilityResponses);
            return Task.FromResult(response);
        }



    }
}
