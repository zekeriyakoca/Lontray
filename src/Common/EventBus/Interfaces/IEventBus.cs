﻿using EventBus.Dtos;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IEventBus
    {
        void Publish(BaseQueueItemDto item, QueueNameEnum queueName);

        void Subscribe(QueueNameEnum queueName, AsyncEventHandler<BasicDeliverEventArgs> onMessageReceived);
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
