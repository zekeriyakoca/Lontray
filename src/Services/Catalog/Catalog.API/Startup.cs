using Autofac;
using Autofac.Extensions.DependencyInjection;
using Catalog.API.AppServices;
using Catalog.API.Infrastructure;
using Catalog.API.Infrastructure.Filters;
using Catalog.API.IntegrationEvents.EventHandlers;
using Catalog.API.IntegrationEvents.Events;
using Catalog.API.IntegrationEvents.Services;
using EventBus;
using EventBus.Events;
using EventBus.Events.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Catalog.API
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
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

            services.AddTransient<CatalogContextSeeder>();

            services.AddTransient<ICatalogAppService, CatalogAppService>();

            services.AddTransient<ICatalogIntegrationService, CatalogIntegrationService>();

            services.AddAutoMapper(typeof(Startup));

            services.AddEventBusRabbitMQ(Configuration);

            services.AddCustomSwagger(Configuration)
                    .AddCustomDbContext(Configuration, env);
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

                subscribemethod.Invoke(eventBus, new object[] { @event.Name, "CatalogApi", handler });
            }
        }

        internal static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                //options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Catalog API",
                    Version = "v1",
                    Description = "The Catalog API."
                });
            });

            return services;

        }

        internal static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
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
                    if (env.IsDevelopment()) options.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information); // logs all sql commands to console
                });


            return services;
        }
    }
}
