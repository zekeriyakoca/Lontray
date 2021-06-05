using DomainHelper.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Entities
{
    public class CatalogFeature : Entity
    {
        //TODO : Implement 

        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }
        public CatalogItem Item { get; set; }
    }
}
