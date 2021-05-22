using EventBus.Events;
using EventBus.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class RabbitMQBus : IEventBus, IDisposable
    {
        const string BROKER_NAME = "integration_event_bus";
        private readonly ConcurrentDictionary<string, IntegrationEvent> CurrentEventQueues;
        private readonly int _retryCount;

        private IModel _consumerChannel;
        private IModel consumerChannel
        {
            get
            {
                if (_consumerChannel != null && _consumerChannel.IsOpen)
                    return _consumerChannel;
                else if (_consumerChannel != null)
                    _consumerChannel.Dispose();

                _consumerChannel = CreateConsumerChannel();
                return _consumerChannel;
            }
            set
            {
                this._consumerChannel = value;
            }
        }

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMQBus> _logger;

        public RabbitMQBus(IRabbitMQPersistentConnection persistentConnection, ILogger<RabbitMQBus> logger,
               string queueName = null, int retryCount = 5)
        {
            CurrentEventQueues = new ConcurrentDictionary<string, IntegrationEvent>();
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
            Initialize();
        }

        private void Initialize()
        {
            _persistentConnection.AddListenerForConnectionEstablished((s, e) =>
            {
                InitializeExchange();
            });
        }

        private void InitializeExchange()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using var channel = _persistentConnection.CreateModel();

            _logger.LogTrace("Declaring RabbitMQ exchange to publish event");

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "topic");
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var routingKey = @event.TypeName + ".*";
            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, @event.TypeName);

            using (var channel = _persistentConnection.CreateModel())
            {
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                    channel.BasicPublish(
                        exchange: BROKER_NAME,
                        routingKey: routingKey,
                        mandatory: true, // There should some subscribers. Will throw BrokerUnreachableException if no subscriber 
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        public void Subscribe<TEvent, THandler>(string typeName, string appSuffix, THandler handler) where TEvent : IntegrationEvent where THandler : IIntegrationEventHandler<TEvent>
        {

            var queueName = typeName + "." + appSuffix;
            var routingKey = typeName + ".*";

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            CreateQueueIfNotExist(queueName, routingKey, consumerChannel);

            StartBasicConsume(queueName, async (s, e) =>
            {
                var @event = e.DeserializeBody<TEvent>();
                var policy = RetryPolicy.Handle<Exception>()
                .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Error occured while handling event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time:n1}", ex.Message);
                });

                var policyWithFallback = RetryPolicy.Handle<Exception>()
                     .FallbackAsync((token) =>
                     {
                         _logger.LogError("Couldn't handle the event: {EventId}. Event Details {EventDetails}: ", @event.Id, JsonConvert.SerializeObject(@event));
                         SendAck(e); // We don't want to try forever. Deleting the evet from queue.
                         return Task.CompletedTask;
                     })
                     .WrapAsync(policy);

                await policyWithFallback.ExecuteAsync(async () =>
                {
                    await handler.Handle(@event);
                    SendAck(e);
                });

            });
        }

        public void SendAck(BasicDeliverEventArgs eventArgs)
        {
            consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        public void SendNack(BasicDeliverEventArgs eventArgs)
        {
            consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }

        private void CreateQueueIfNotExist(string queueName, string routingKey, IModel channel)
        {

            if (CurrentEventQueues.ContainsKey(queueName))
            {
                return;
            }

            _logger.LogTrace("Declaring Queue to publish event. QueueName : {QueueName}", queueName);

            channel.QueueDeclare(queue: queueName,
                                       durable: true,
                                       exclusive: false,
                                       autoDelete: false,
                                       arguments: null);

            channel.QueueBind(queueName, BROKER_NAME, routingKey: routingKey);
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();
            channel.BasicQos(0, prefetchCount: 20, false);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        private void StartBasicConsume(string queueName, AsyncEventHandler<BasicDeliverEventArgs> onMessageReceived)
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            var consumer = new AsyncEventingBasicConsumer(consumerChannel);

            consumer.Received += onMessageReceived;

            var consumerTag = consumerChannel.BasicConsume(
             queue: queueName,
             autoAck: false,
             consumer: consumer);


        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
        }
        // This event handler is just for test
        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

            try
            {
                if (message.ToLowerInvariant().Contains("throw-fake-exception"))
                {
                    throw new InvalidOperationException($"Fake exception requested: \"{message}\"");
                }

                //await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            // TODO : handled with a Dead Letter Exchange (DLX). 
            SendAck(eventArgs);
        }


    }
}
