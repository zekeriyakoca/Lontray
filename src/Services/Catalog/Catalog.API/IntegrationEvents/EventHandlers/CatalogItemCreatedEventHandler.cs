using Catalog.API.Infrastructure;
using Catalog.API.IntegrationEvents.Events;
using EventBus.Events;
using EventBus.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.EventHandlers
{
    public record CatalogItemCreatedEventHandler : IIntegrationEventHandler<CatalogItemCreatedEvent>
    {
        private readonly CatalogContext _catalogContext;
        private readonly ILogger<CatalogItemCreatedEventHandler> _logger;

        public CatalogItemCreatedEventHandler(
            CatalogContext catalogContext,
            ILogger<CatalogItemCreatedEventHandler> logger)
        {
            _catalogContext = catalogContext;
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task Handle(CatalogItemCreatedEvent @event)
        {
            using (LogContext.PushProperty("IntegrationEventContext", $"{@event.Id}-{Program.AppName}"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                _logger.LogInformation("----- Handling integration event: {IntegrationEventId} at {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
            }
        }
    }
}
