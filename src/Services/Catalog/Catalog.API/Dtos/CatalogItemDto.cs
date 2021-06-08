using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Dtos
{
    public class CatalogItemDto
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureFileName { get; set; }

        public string PictureUri { get; set; }

        public CatalogBrandDto Brand { get; set; }

        public CatalogTypeDto Type { get; set; }

        public int AvailableStock { get; set; }

        public bool OnReorder { get; set; }

        public string Tags { get; set; }

        public IEnumerable<CatalogFeatureDto> Features { get; set; }



    }
}
