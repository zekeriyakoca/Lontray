using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Grpc
{
    public class CatalogService : Catalog.CatalogBase
    {
        private readonly ILogger<CatalogService> logger;
        public CatalogService(ILogger<CatalogService> logger)
        {
            this.logger = logger;
        }

        public override Task<CheckAvailibilityResponse> ValidateBasket(CheckAvailibilityRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }



    }
}
