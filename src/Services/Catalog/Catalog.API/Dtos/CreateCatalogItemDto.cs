using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Dtos
{
    public class CreateCatalogItemDto
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string PictureFileName { get; set; }

        [Required]
        public string PictureUri { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int TypeId { get; set; }

        [Required]
        public int AvailableStock { get; set; }

        public int RestockThreshold { get; set; }

        public int MaxStockThreshold { get; set; }
    }
}
