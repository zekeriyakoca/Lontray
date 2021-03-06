using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Events.Interfaces;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Ordering.API.Infrastructure.Filters;
using Ordering.Infrastructure;
using Ordering.Infrastructure.AutofacModules;
using Ordering.Infrastructure.CQRS;
using Ordering.Infrastructure.Filters;
using Ordering.Infrastructure.Services;
using OrderingGrpc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
                    .ConfigureAuthService(Configuration)
                    .AddCustomDbContext(Configuration, env);

            services.AddHttpContextAccessor();

            services.AddGrpc();
            services.AddTransient<OrderingContextSeeder>();
            services.AddTransient<IQueryExecuter, QueryExecuter>();
            services.AddTransient<ICommandExecuter, CommandExecuter>();
            services.AddTransient<IOrderingIntegrationService, OrderingIntegrationService>();

            var hcBuilder = services.AddHealthChecks();

            hcBuilder
              .AddCheck("self", () => HealthCheckResult.Healthy())
              .AddDbContextCheck<OrderingContext>(
                  name: "OrderingDB-check",
                  tags: new string[] { "orderingdb" });

            hcBuilder
                   .AddRabbitMQ(
                       $"amqp://{Configuration["RabbitMQ:EventBusConnection"]}",
                       name: "ordering-rabbitmqbus-check",
                       tags: new string[] { "rabbitmqbus" });

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
                app.UseSwaggerUI(setup =>
                {
                    setup.SwaggerEndpoint($"/swagger/v1/swagger.json", "Ordering.API V1");
                    setup.OAuthClientId("orderingswaggerui");
                    setup.OAuthAppName("Ordering Swagger UI");
                });
            }

            //app.UseHttpsRedirection(); // Check GPRC port configuration before uncomment

            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapGet("/_proto/", async ctx =>
                {
                    ctx.Response.ContentType = "text/plain";
                    using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "ordering.proto"), FileMode.Open, FileAccess.Read);
                    using var sr = new StreamReader(fs);
                    while (!sr.EndOfStream)
                    {
                        var line = await sr.ReadLineAsync();
                        if (line != "/* >>" || line != "<< */")
                        {
                            await ctx.Response.WriteAsync(line);
                        }
                    }
                });
                endpoints.MapGrpcService<OrderingService>();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
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
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "The Ordering API",
                    Version = "v1",
                    Description = "The Ordering API"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                            TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                            Scopes = new Dictionary<string, string>()
                            {
                                { "orderingApi.all", "Ordering API" }
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
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

        internal static IServiceCollection ConfigureAuthService(this IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            var identityUrl = configuration.GetValue<string>("IdentityUrl");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = identityUrl;
                options.RequireHttpsMetadata = false;
                options.Audience = "orderingApi";
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("manage_access", policy =>
                    policy.RequireClaim("scope", "orderingApi.all"));
            });

            return services;
        }

        //internal static void StartDomainHandlers(this IServiceCollection services)
        //{
        //    DomainEvents.Init(services);
        //}
    }
}
