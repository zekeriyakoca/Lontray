using EventBus.Events;

namespace Basket.API.IntegrationEvents.Events
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
