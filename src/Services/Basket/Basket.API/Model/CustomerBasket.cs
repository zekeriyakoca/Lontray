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

        public CustomerBasket MergeFrom(CustomerBasket oldBasket)
        {
            if (oldBasket?.Items.Count < 1)
                return this;
            foreach (var item in oldBasket.Items)
            {
                var current = this.Items.SingleOrDefault(i => item.ProductId == i.ProductId);
                if (current == default)
                {
                    this.Items.Add(item);
                }
                else
                {
                    current.Quantity += current.Quantity;
                    // Price etc will be validated in BFF;
                }
            }
            return this;

        }

    }

}
