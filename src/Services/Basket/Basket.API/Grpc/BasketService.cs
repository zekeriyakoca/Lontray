using Basket.API.Infrastructure.Repositories;
using Basket.API.Model;
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
        private readonly IBasketRepository basketRepository;

        public BasketService(ILogger<BasketService> logger, IBasketRepository basketRepository)
        {
            this.logger = logger;
            this.basketRepository = basketRepository;
        }

        // TODO : To be tested
        public override async Task<CustomerBasketResponse> GetBasketById(BasketRequest request, ServerCallContext context)
        {
            var basket = await basketRepository.GetBasketAsync(request.Buyerid);
            return basket.ToCustomerBasketResponse();
        }

        // TODO : To be tested
        public override async Task<CustomerBasketResponse> UpdateBasket(CustomerBasketRequest request, ServerCallContext context)
        {
            var updatedBasket = await basketRepository.UpdateBasketAsync(new CustomerBasket(request.Buyerid)
            {
                Items = request.Items.Select(i => new BasketItem
                {
                    Id = i.Id,
                    OldUnitPrice = Convert.ToDecimal(i.Oldunitprice),
                    PictureUrl = i.Pictureurl,
                    ProductId = i.Productid,
                    ProductName = i.Productname,
                    Quantity = i.Quantity,
                    UnitPrice = Convert.ToDecimal(i.Unitprice)
                }).ToList()
            });
            return updatedBasket.ToCustomerBasketResponse();
        }

        public override Task<ValidateBasketResponse> ValidateBasket(ValidateBasketRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }

    public static class MappingExtension
    {
        public static CustomerBasketResponse ToCustomerBasketResponse(this CustomerBasket basket)
        {
            var response = new CustomerBasketResponse() { Buyerid = basket.CustomerId };
            foreach (var item in basket.Items)
            {
                response.Items.Add(new BasketItemResponse()
                {
                    Id = item.Id,
                    Oldunitprice = Decimal.ToDouble(item.OldUnitPrice),
                    Pictureurl = item.PictureUrl,
                    Productid = item.ProductId,
                    Productname = item.ProductName,
                    Quantity = item.Quantity,
                    Unitprice = Decimal.ToDouble(item.UnitPrice)
                });
            }
            return response;
        }
    }
}
