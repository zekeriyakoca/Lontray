using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.BFF.Shopping
{
    public class UrlConfig
    {
        public string Identity { get; set; }
        public string Ordering { get; set; }
        public string Basket { get; set; }
        public string Catalog { get; set; }

        public string BasketGrpc { get; set; }
        public string CatalogGrpc { get; set; }
        public string OrderingGrpc { get; set; }
    }
}
