using Autofac;
using Autofac.Extensions.DependencyInjection;
using Basket.API.Infrastructure.Filters;
using Basket.API.Infrastructure.Repositories;
using EventBus;
using EventBus.Events.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Basket.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter));
            }).AddNewtonsoftJson();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddOptions();
            services.Configure<BasketSettings>(Configuration);

            services.AddTransient<IBasketRepository, RedisBasketRepository>();

            services.AddCache(Cache.Enum.CachingServiceEnum.InMemory);
            //services.AddCache(Cache.Enum.CachingServiceEnum.Redis); // Activate after Redis is ready

            services.AddEventBusRabbitMQ(Configuration);

            services.AddCustomSwagger(Configuration);
        }

        //Configure Autofac Container
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //Add IntegrationHandlers to Container
            var currentAssembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(t => t.GetInterfaces().Contains(typeof(IIntegrationEventHandler)))
                .PreserveExistingDefaults()
                .AsImplementedInterfaces();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });

            app.ConfigureIntegrationEvents();
        }
    }
    internal static class ServiceProviderExtensions
    {
        internal static void ConfigureIntegrationEvents(this IApplicationBuilder app)
        {
            var autofacContainer = app.ApplicationServices.GetAutofacRoot();

            var eventBus = autofacContainer.Resolve<IEventBus>();
            var integrationEventHandlers = autofacContainer.ResolveOptional<IEnumerable<IIntegrationEventHandler>>();
            foreach (var handler in integrationEventHandlers)
            {
                var @event = handler.GetType().GetInterfaces().First().GenericTypeArguments.First();

                var subscribemethod = typeof(IEventBus)
                    .GetMethod("Subscribe")
                    .MakeGenericMethod(@event, handler.GetType());

                subscribemethod.Invoke(eventBus, new object[] { @event.Name, "BasketApi", handler });
            }
        }

        internal static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Basket API",
                    Version = "v1",
                    Description = "The Basket API."
                });
            });

            return services;

        }
    }
}
