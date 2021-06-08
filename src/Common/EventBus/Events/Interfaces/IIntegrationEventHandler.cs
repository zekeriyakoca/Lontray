using System.Threading.Tasks;

namespace EventBus.Events.Interfaces
{
    public interface IIntegrationEventHandler<TIntegrationEvent> : IIntegrationEventHandler
       where TIntegrationEvent : IntegrationEvent
    {
        Task Handle(TIntegrationEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
