using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Basket.API.Model
{
    public class BasketItem : IValidatableObject
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal OldUnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Quantity < 1)
            {
                results.Add(new ValidationResult($"Invalid number of units, ProductId: {ProductId}", new[] { "Quantity" }));
            }

            return results;
        }
    }
}
