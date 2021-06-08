using EventBus.Events;

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
