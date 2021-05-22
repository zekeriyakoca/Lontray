using Ordering.Domain.Common;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.Application.DomainEventHandlers
{
    public class OrderCreatedDomainEventHandler : IHandler<OrderCreatedDomainEvent>
    {
        public Task Handle(OrderCreatedDomainEvent param)
        {
            throw new NotImplementedException();
        }
    }
}
