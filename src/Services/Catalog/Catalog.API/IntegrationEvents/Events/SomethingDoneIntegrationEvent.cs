using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.Events
{
    public record SomethingDoneIntegrationEvent : IntegrationEvent
    {
        public SomethingDoneIntegrationEvent() : base() { }

        public SomethingDoneIntegrationEvent(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}
