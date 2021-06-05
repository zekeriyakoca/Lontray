using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketGrpc
{
    public class BasketService : Basket.BasketBase
    {
        private readonly ILogger<BasketService> logger;
        public BasketService(ILogger<BasketService> logger)
        {
            this.logger = logger;
        }

        public override Task<CustomerBasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<CustomerBasketResponse> UpdateBasket(CustomerBasketRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<ValidateBasketResponse> ValidateBasket(ValidateBasketRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
