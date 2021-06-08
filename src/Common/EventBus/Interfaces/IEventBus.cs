using EventBus.Events;
using EventBus.Events.Interfaces;
using RabbitMQ.Client.Events;

namespace EventBus
{
    public interface IEventBus
    {
        void Subscribe<TEvent, THandler>(string typeName, string appSuffix, THandler handler) where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>;

        void Publish(IntegrationEvent @event);
        void SendAck(BasicDeliverEventArgs eventArgs);
        void SendNack(BasicDeliverEventArgs eventArgs);
        //void Subscribe<T, TH>()
        //    where T : IntegrationEvent
        //    where TH : IIntegrationEventHandler<T>;

        //void SubscribeDynamic<TH>(string eventName)
        //    where TH : IDynamicIntegrationEventHandler;

        //void UnsubscribeDynamic<TH>(string eventName)
        //    where TH : IDynamicIntegrationEventHandler;

        //void Unsubscribe<T, TH>()
        //    where TH : IIntegrationEventHandler<T>
        //    where T : IntegrationEvent;
    }
}
