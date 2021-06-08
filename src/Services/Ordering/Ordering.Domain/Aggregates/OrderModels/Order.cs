using Ordering.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordering.Domain.Aggregates
{
    public class Order : Entity, IAggregateRoot
    {
        [Column("OrderDate")]
        [Required]
        public DateTime OrderDate { get; private set; }

        public Address InvoiceAddress { get; private set; }
        public Address ShippingAddress { get; private set; }

        [ForeignKey(nameof(Buyer))]
        [Required]
        public int BuyerId { get; private set; }
        public Buyer Buyer { get; private set; }

        [ForeignKey(nameof(OrderStatus))]
        [Required]
        public int OrderStatusId { get; private set; }
        public OrderStatus OrderStatus { get; private set; }

        public string Description { get; set; }
        public bool IsDraft { get; set; }

        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public static Order NewDraft()
        {
            var order = new Order();
            order.IsDraft = true;
            return order;
        }

        protected Order()
        {
            _orderItems = new List<OrderItem>();
            IsDraft = false;
        }

    }
}
