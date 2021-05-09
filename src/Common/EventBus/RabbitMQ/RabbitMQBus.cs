using EventBus.Dtos;
using EventBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventBus
{
    public class RabbitMQBus : IEventBus, IDisposable
    {
        const string BROKER_NAME = "image_process_bus";

        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly ILogger<RabbitMQBus> _logger;

        private readonly int _retryCount;
        private bool queuesInitialized = false;
        private IModel _consumerChannel;

        public RabbitMQBus(IRabbitMQPersistentConnection persistentConnection, ILogger<RabbitMQBus> logger,
               string queueName = null, int retryCount = 5)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _retryCount = retryCount;
            InitializeQueue();
            _consumerChannel = CreateConsumerChannel();

        }

        public void CreateQueue(QueueNameEnum queueName, IModel channel)
        {
            _logger.LogTrace("Declaring Queue to publish event. QueueName : {QueueName}", queueName);

            channel.QueueDeclare(queue: queueName.Value,
                                       durable: false,
                                       exclusive: false,
                                       autoDelete: false,
                                       arguments: null);

            channel.QueueBind(queueName.Value, BROKER_NAME, routingKey: queueName.Value);
        }
        private void InitializeQueue()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }
            using (var channel = _persistentConnection.CreateModel())
            {
                CreateInitialQueues(channel);
            }
        }

        private void CreateInitialQueues(IModel channel)
        {
            if (queuesInitialized)
                return;

            _logger.LogTrace("Declaring RabbitMQ exchange to publish event");

            channel.ExchangeDeclare(exchange: BROKER_NAME, type: "direct");

            _logger.LogTrace("Declaring RabbitMQ initial queues");

            CreateQueue(QueueNameEnum.ImageToCompress, channel);
            CreateQueue(QueueNameEnum.ImageCompressed, channel);
            //CreateQueue(QueueNameEnum.MailSent, channel);

            queuesInitialized = true;
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            _logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = _persistentConnection.CreateModel();

            CreateInitialQueues(channel);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
            };

            return channel;
        }

        public void Publish(BaseQueueItemDto item, QueueNameEnum queueName)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = RetryPolicy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    _logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", item.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            _logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", item.Id, queueName.Value);

            using (var channel = _persistentConnection.CreateModel())
            {
                var message = JsonConvert.SerializeObject(item);
                var body = Encoding.UTF8.GetBytes(message);

                policy.Execute(() =>
                {
                    var properties = channel.CreateBasicProperties();
                    properties.DeliveryMode = 2; // persistent
                                                 //properties.ContentType = "text/plain"; // text/plain is default
                                                 //properties.Expiration = "36000000";

                    _logger.LogTrace("Publishing event to RabbitMQ: {EventId}", item.Id);

                    channel.BasicPublish(
                        exchange: BROKER_NAME,
                        routingKey: queueName.Value,
                        mandatory: true,
                        basicProperties: properties,
                        body: body);
                });
            }
        }

        public void Subscribe(QueueNameEnum queueName, AsyncEventHandler<BasicDeliverEventArgs> onMessageReceived)
        {
            if (!queuesInitialized)
                InitializeQueue();

            StartBasicConsume(queueName, onMessageReceived);
        }

        public void SendAck(BasicDeliverEventArgs eventArgs)
        {
            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }
        public void SendNack(BasicDeliverEventArgs eventArgs)
        {
            _consumerChannel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }

        public void Dispose()
        {
            if (_consumerChannel != null)
            {
                _consumerChannel.Dispose();
            }
        }

        private void StartBasicConsume(QueueNameEnum queueName, AsyncEventHandler<BasicDeliverEventArgs> onMessageReceived)
        {
            _logger.LogTrace("Starting RabbitMQ basic consume");

            _consumerChannel.QueueDeclare(queue: queueName.Value,
                                       durable: false,
                                       exclusive: false,
                                       autoDelete: false,
                                       arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_consumerChannel);

            consumer.Received += onMessageReceived;

            var consumerTag = _consumerChannel.BasicConsume(
                queue: queueName.Value,
                autoAck: false,
                consumer: consumer);
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
