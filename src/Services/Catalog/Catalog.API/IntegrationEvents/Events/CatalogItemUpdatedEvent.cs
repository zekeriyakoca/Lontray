
using EventBus.Events;
using System;

namespace Catalog.API.IntegrationEvents.Events
{
    public record CatalogItemUpdatedEvent : IntegrationEvent
    {
        public int CatalogItemId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int BrandId { get; set; }

        public int TypeId { get; set; }

        public int AvailableStock { get; set; }

        public CatalogItemUpdatedEvent()
        { }
    }
}
