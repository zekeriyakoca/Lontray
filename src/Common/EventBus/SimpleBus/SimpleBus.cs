using EventBus;
using EventBus.Dtos;
using EventBus.Events;
using EventBus.Events.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.SimpleBus
{
    //TODO : Refactor after creating Unit Tests
    //TODO : Rewrite after RabbitMQ implementation
    public class SimpleBus : IEventBus, IDisposable
    {
        private static ConcurrentBag<SimpleQueue> Queues { get; set; }
        private readonly ILogger<SimpleBus> logger;
        private readonly IHostingEnvironment hostingEnvironment;
        private bool disposed;

        public SimpleBus(ILogger<SimpleBus> logger, IHostingEnvironment hostingEnvironment)
        {
            this.logger = logger;
            this.hostingEnvironment = hostingEnvironment;
            Queues = new ConcurrentBag<SimpleQueue>();
            FetchQueueFromDisk();
            InitializeQueues();
        }

        public void Subscribe<TEvent, THandler>(TEvent @event, string appSuffix, THandler handler)
         where TEvent : IntegrationEvent
         where THandler : IIntegrationEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }

        public void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(QueueNameEnum queueName, AsyncEventHandler<BasicDeliverEventArgs> onMessageReceived)
        {
            var queue = Queues.SingleOrDefault(q => q.Name == queueName.Value);
            if (queue == default)
            {
                throw new ArgumentNullException("There is no such a queue");
            }
            queue.AddListener(onMessageReceived);
        }

        public void Publish(BaseQueueItemDto item, QueueNameEnum queueName)
        {
            var queue = SetToQueue(item, queueName);
            RunNext(queueName.Value, queue);
        }

        private async Task RunNext(string queueName, SimpleQueue queue)
        {
            (var nextTask, var subscriber) = queue.GetNext();
            if (subscriber == null)
                return;

            var del = subscriber?.Value;
            await del?.Invoke(null, new BasicDeliverEventArgs { ConsumerTag = queueName, RoutingKey = subscriber.Id, Body = Encoding.UTF8.GetBytes(nextTask) }).ContinueWith(t =>
            {
                SendNack(new BasicDeliverEventArgs { ConsumerTag = queueName, RoutingKey = del.GetHashCode().ToString() });
            }, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SendAck(BasicDeliverEventArgs eventArgs)
        {
            var queueName = eventArgs.ConsumerTag;
            var queue = GetQueue(queueName);
            queue.UpdateStatus(eventArgs.RoutingKey);
            RunNext(queueName, queue)
                .ContinueWith(t =>
                    {

                    }, TaskContinuationOptions.OnlyOnFaulted)
                .Wait();
        }

        public void SendNack(BasicDeliverEventArgs eventArgs)
        {
            logger.LogError("Error occured while processing and queued task");

            var queueName = eventArgs.ConsumerTag;
            var queue = GetQueue(queueName);
            var queueItem = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            if (!String.IsNullOrWhiteSpace(queueItem))
            {
                //TODO : Implement requeuing after implementation of retry policy
                logger.LogError("Requeuing failed task...");
                //queue.Queue.Enqueue(JsonConvert.SerializeObject(queueItem));
            }

            queue.UpdateStatus(eventArgs.RoutingKey);
            RunNext(queueName, queue).Wait();
        }


        private void InitializeQueues()
        {
            //CreateQueue(QueueNameEnum.ImageToCompress);
            //CreateQueue(QueueNameEnum.ImageCompressed);
        }

        private void CreateQueue(QueueNameEnum queueName)
        {
            if (Queues.Any(q => q.Name == queueName.Value))
                return;

            Queues.Add(new SimpleQueue { Name = queueName.Value });
        }

        private SimpleQueue SetToQueue(BaseQueueItemDto item, QueueNameEnum queueName)
        {
            SimpleQueue queue = GetQueue(queueName.Value);
            queue.Queue.Enqueue(JsonConvert.SerializeObject(item));
            return queue;
        }

        private static SimpleQueue GetQueue(string queueName)
        {
            var queue = Queues.SingleOrDefault(q => q.Name == queueName);
            if (queue == default)
            {
                throw new ArgumentNullException("There is no such a queue");
            }

            return queue;
        }

        private void FetchQueueFromDisk()
        {
            var path = Path.Combine(hostingEnvironment.WebRootPath ?? hostingEnvironment.ContentRootPath, "EventBusQueues.json");
            if (!File.Exists(path))
                return;

            using StreamReader outputFile = new StreamReader(path);
            var json = outputFile.ReadToEnd();
            if (!String.IsNullOrWhiteSpace(json))
                Queues = JsonConvert.DeserializeObject<ConcurrentBag<SimpleQueue>>(json);
        }


        private class SimpleQueue
        {
            private List<EventHandlerDelegate> EventHandlerDelegates;
            private List<string> Subscribers { get; set; }

            public SimpleQueue()
            {
                Queue = new ConcurrentQueue<string>();
                EventHandlerDelegates = new List<EventHandlerDelegate>();
            }

            public string Name { get; set; }
            public ConcurrentQueue<String> Queue { get; }

            public void AddListener(AsyncEventHandler<BasicDeliverEventArgs> handler)
            {
                EventHandlerDelegates.Add(new EventHandlerDelegate(handler));
            }
            public (string, EventHandlerDelegate) GetNext()
            {
                Queue.TryDequeue(out string task);
                if (task == null)
                {
                    return default;
                }
                var nextSubscriber = EventHandlerDelegates.FirstOrDefault(d => d.IsEligible);
                if (nextSubscriber == null)
                {
                    Queue.Enqueue(task);
                    return default;
                }
                nextSubscriber.IsEligible = false;
                return (task, nextSubscriber);
            }

            public void UpdateStatus(string id)
            {
                var item = EventHandlerDelegates.SingleOrDefault(d => d.Id == id);
                if (item != null)
                {
                    item.IsEligible = true;
                }
            }

        }

        private class EventHandlerDelegate
        {
            public EventHandlerDelegate(AsyncEventHandler<BasicDeliverEventArgs> @delegate)
            {
                Value = @delegate;
                Id = Guid.NewGuid().ToString();
            }
            public AsyncEventHandler<BasicDeliverEventArgs> Value { get; set; }
            public string Id { get; set; }
            public bool IsEligible { get; set; } = true;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
            StreamWriter outputFile = new StreamWriter(Path.Combine(hostingEnvironment.WebRootPath ?? hostingEnvironment.ContentRootPath, "EventBusQueues.json"));
            outputFile.Write(JsonConvert.SerializeObject(Queues));
            outputFile.Dispose();

        }

     
    }


}
