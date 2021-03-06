using RabbitMQ.Client;
using System;

namespace EventBus
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();

        void AddListenerForConnectionEstablished(EventHandler<EventArgs> handler);
    }
}
