using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Filters;
using Catalog.API.IntegrationEvents.EventHandlers;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.IntegrationEvents.Services;
using EventBus;
using EventBus.Events.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;

namespace Catalog.API
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
            services.Configure<CatalogSettings>(Configuration);

            var connectionString = Configuration["ConnectionString"];
            var migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddDbContext<CatalogContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    (sqlOptions) =>
                    {
                        sqlOptions.MigrationsAssembly(migrationAssembly);
                        sqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), default);
                    });
            });
            services.AddTransient<CatalogContextSeeder>();

            services.AddTransient<ICatalogIntegrationService, CatalogIntegrationService>();

            services.AddEventBusRabbitMQ(Configuration);

            testRabbitMq(services, Configuration);

            services.AddCustomSwagger(Configuration)
                    .AddCustomDbContext(Configuration);
        }

        private void testRabbitMq(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IIntegrationEventHandler<TestIntegrationEvent>, SomethingDoneIntegrationEventHandler>();

            using var scope = services.BuildServiceProvider().CreateScope();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
            var @event = new TestIntegrationEvent("Test Event Name");
            var eventHandler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TestIntegrationEvent>>();
            eventBus.Subscribe<TestIntegrationEvent, IIntegrationEventHandler<TestIntegrationEvent>>(@event, "OrderApi", eventHandler);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog.API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }

    internal static class ServiceProviderExtensions
    {
        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Catalog API",
                    Version = "v1",
                    Description = "The Catalog API."
                });
            });

            return services;

        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<CatalogContext>(options =>
                {
                    options.UseSqlServer(configuration["ConnectionString"],
                                         sqlServerOptionsAction: sqlOptions =>
                                         {
                                             sqlOptions.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                                             sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                                         });
                });


            return services;
        }
    }
}
