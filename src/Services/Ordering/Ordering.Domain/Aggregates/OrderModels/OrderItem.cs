using Ordering.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Aggregates
{
    [Table("OrderItems")]
    public class OrderItem : Entity
    {
        [Required]
        public string ProductName { get; private set; }
        public string PictureUrl { get; private set; }
        [Required]
        public decimal UnitPrice { get; private set; }
        [Required]
        public decimal Discount { get; private set; }
        [Required]
        public int Units { get; private set; }
        [Required]
        public int ProductId { get; private set; }

        [Required]
        [ForeignKey(nameof(Order))]
        public int OrderId { get; private set; }
        public Order Order { get; private set; }
    }
}
