using Ordering.API.Application;
using Ordering.API.Application.IntegrationEvents;
using Ordering.Domain.Common;
using Ordering.Domain.Events;
using Ordering.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Ordering.Application.DomainEventHandlers
{
    public class OrderCreatedDomainEventHandler : IHandler<OrderCreatedDomainEvent>
    {
        private readonly IOrderingIntegrationService integrationService;

        public OrderCreatedDomainEventHandler(IOrderingIntegrationService integrationService)
        {
            this.integrationService = integrationService;
        }
        public Task Handle(OrderCreatedDomainEvent param)
        {
            integrationService.PublishEvent(new OrderCreatedIntegrationEvent());
            return Task.CompletedTask;
        }
    }
}
