using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Basket.API.Model
{
    public class CustomerBasket
    {
        public string CustomerId { get; set; }

        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public CustomerBasket()
        {

        }

        public CustomerBasket(string customerId)
        {
            CustomerId = customerId;
        }

    }

}
