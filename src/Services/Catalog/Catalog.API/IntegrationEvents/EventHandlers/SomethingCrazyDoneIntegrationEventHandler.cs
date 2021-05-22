using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents.Events;
using EventBus.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.EventHandlers
{
    public record SomethingCrazyDoneIntegrationEventHandler : IIntegrationEventHandler<SomethingCrazyDoneIntegrationEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly ILogger<SomethingCrazyDoneIntegrationEventHandler> _logger;

        public SomethingCrazyDoneIntegrationEventHandler(
            CatalogContext catalogContext,
            ILogger<SomethingCrazyDoneIntegrationEventHandler> logger)
        {
            _catalogContext = catalogContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(SomethingCrazyDoneIntegrationEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
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
