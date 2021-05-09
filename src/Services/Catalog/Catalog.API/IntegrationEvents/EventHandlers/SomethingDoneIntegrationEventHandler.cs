using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents.Events;
using EventBus.Events;
using EventBus.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.EventHandlers
{  
    public record SomethingDoneIntegrationEventHandler : IIntegrationEventHandler<TestIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly ILogger<SomethingDoneIntegrationEventHandler> _logger;

        public SomethingDoneIntegrationEventHandler(
            CatalogContext catalogContext,
            ILogger<SomethingDoneIntegrationEventHandler> logger)
        {
            _catalogContext = catalogContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(TestIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                //we're not blocking stock/inventory
                //foreach (var orderStockItem in @event.OrderStockItems)
                //{
                //    var catalogItem = _catalogContext.CatalogItems.Find(orderStockItem.ProductId);

                //    catalogItem.RemoveStock(orderStockItem.Units);
                //}

                //await _catalogContext.SaveChangesAsync();

            }
        }
    }
}
