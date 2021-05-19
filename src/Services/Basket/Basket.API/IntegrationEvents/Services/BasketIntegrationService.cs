using EventBus;
using EventBus.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API.IntegrationEvents.Services
{
    public interface IBasketIntegrationService
    {
        void PublishEvent(IntegrationEvent @event);
    }

    public class BasketIntegrationService : IBasketIntegrationService
    {
        private readonly IEventBus eventBus;
        private readonly ILogger<BasketIntegrationService> logger;

        public BasketIntegrationService(IEventBus eventBus, ILogger<BasketIntegrationService> logger)
        {
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public void PublishEvent(IntegrationEvent @event)
        {
            try
            {
                logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);

                eventBus.Publish(@event);

                logger.LogInformation("----- Published integration event: {IntegrationEventId_published} from {AppName})", @event.Id, Program.AppName);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occured while publishing event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", @event.Id, Program.AppName, @event);
            }

        }
    }
}
