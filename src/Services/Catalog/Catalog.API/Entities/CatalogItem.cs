using DomainHelper.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Entities
{
    public class CatalogItem : EntityAudited
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureFileName { get; set; }

        public string PictureUri { get; set; }

        public CatalogItem() { }

        [ForeignKey(nameof(Brand))]
        public int BrandId { get; set; }
        public CatalogBrand Brand { get; set; }

        [ForeignKey(nameof(Type))]
        public int TypeId { get; set; }
        public CatalogType Type { get; set; }

        // Quantity in stock
        public int AvailableStock { get; set; }

        // Available stock at which we should reorder
        public int RestockThreshold { get; set; }


        // Maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses)
        public int MaxStockThreshold { get; set; }

        /// <summary>
        /// True if item is on reorder
        /// </summary>
        public bool OnReorder { get; set; }


    }
}
