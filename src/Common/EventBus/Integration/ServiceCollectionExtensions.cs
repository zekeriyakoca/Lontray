﻿using EventBus;
using EventBus.SimpleBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddEventBusRabbitMQ(this IServiceCollection services, IConfiguration Configuration)
        {

            var retryCount = Configuration["RabbitMQ:EventBusRetryCount"] ?? "5";

            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = Configuration["RabbitMQ:EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                factory.UserName = Configuration["RabbitMQ:EventBusUserName"] ?? default;
                factory.Password = Configuration["RabbitMQ:EventBusPassword"] ?? default;

                return new DefaultRabbitMQPersistentConnection(factory, logger, int.Parse(retryCount));
            });

            services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
            {
                var subscriptionClientName = Configuration["RabbitMQ:SubscriptionClientName"];

                var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                var logger = sp.GetRequiredService<ILogger<RabbitMQBus>>();

                return new RabbitMQBus(rabbitMQPersistentConnection, logger, subscriptionClientName, int.Parse(retryCount));
            });


        }

        public static void AddEventBusSimple(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton<IEventBus, SimpleBus>();
        }
    }
}

