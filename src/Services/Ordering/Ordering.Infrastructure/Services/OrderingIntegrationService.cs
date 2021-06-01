using EventBus;
using EventBus.Events;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Services
{
    public interface IOrderingIntegrationService
    {
        void PublishEvent(IntegrationEvent @event);
    }

    public class OrderingIntegrationService : IOrderingIntegrationService
    {
        private readonly IEventBus eventBus;
        private readonly ILogger<OrderingIntegrationService> logger;

        public OrderingIntegrationService(IEventBus eventBus, ILogger<OrderingIntegrationService> logger)
        {
            this.eventBus = eventBus;
            this.logger = logger;
        }

        public void PublishEvent(IntegrationEvent @event)
        {
            try
            {
                logger.LogInformation("----- Publishing integration event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", @event.Id, "Ordering");

                eventBus.Publish(@event);

                logger.LogInformation("----- Published integration event: {IntegrationEventId_published} from {AppName})", @event.Id, "Ordering");

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occured while publishing event: {IntegrationEventId_published} from {AppName} - ({@IntegrationEvent})", @event.Id, "Ordering", @event);
            }

        }
    }
}
