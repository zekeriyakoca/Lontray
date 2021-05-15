
using EventBus.Events;
using System;

namespace Catalog.API.IntegrationEvents.Events
{
    public record CatalogItemCreatedEvent : IntegrationEvent
    {
        public int CatalogItemId { get; set; }

        public int? LastCreaterUserId { get; set; }
        public DateTime? LastCreationTime { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string PictureFileName { get; set; }

        public string PictureUri { get; set; }

        public int BrandId { get; set; }

        public int TypeId { get; set; }

        public int AvailableStock { get; set; }

        public int RestockThreshold { get; set; }

        public int MaxStockThreshold { get; set; }

        public CatalogItemCreatedEvent()
        { }
    }
}
