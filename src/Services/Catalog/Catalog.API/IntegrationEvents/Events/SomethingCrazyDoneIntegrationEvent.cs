using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.IntegrationEvents.Events
{
    public record SomethingCrazyDoneIntegrationEvent : IntegrationEvent
    {
        public SomethingCrazyDoneIntegrationEvent() : base() { }

        public SomethingCrazyDoneIntegrationEvent(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
}
