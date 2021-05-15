using Basket.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.Infrastructure.Repositories
{
    public interface IBasketRepository
    {

        Task<CustomerBasket> GetBasketAsync(string customerId);
        IEnumerable<string> GetUsers();
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task DeleteBasketAsync(string id);
    }
}
