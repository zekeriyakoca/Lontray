using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Events.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ordering.Infrastructure;
using Ordering.Infrastructure.AutofacModules;
using Ordering.Infrastructure.CQRS;
using Ordering.Infrastructure.Filters;
using Ordering.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ordering.API
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

            services.AddOptions()
                .Configure<OrderingSettings>(Configuration);


            services.AddEventBusRabbitMQ(Configuration)
                    .AddCustomSwagger(Configuration)
                    .AddCustomDbContext(Configuration, env);

            services.AddHttpContextAccessor();

            services.AddTransient<OrderingContextSeeder>();
            services.AddTransient<IQueryExecuter, QueryExecuter>();
            services.AddTransient<ICommandExecuter, CommandExecuter>();
            services.AddTransient<IOrderingIntegrationService, OrderingIntegrationService>();

            InitializeAdditionalContainer(services);
        }

        private static void InitializeAdditionalContainer(IServiceCollection services)
        {
            // There is no need this class anymore since we are handling domain events during SaveChanges method of EF.
            // This approach also provide access to DI
            //services.StartDomainHandlers();
        }

        //Configure Autofac Container
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
            }

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

    internal static class StartupExtensions
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

                subscribemethod.Invoke(eventBus, new object[] { @event.Name, "OrderingApi", handler });
            }
        }

        internal static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                //options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ordering API",
                    Version = "v1",
                    Description = "The Ordering API."
                });
            });

            return services;

        }

        internal static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<OrderingContext>(options =>
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

        //internal static void StartDomainHandlers(this IServiceCollection services)
        //{
        //    DomainEvents.Init(services);
        //}
    }
}
