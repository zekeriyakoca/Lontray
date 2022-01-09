using Autofac;
using Autofac.Extensions.DependencyInjection;
using Basket.API.Infrastructure.Filters;
using Basket.API.Infrastructure.Repositories;
using Basket.API.IntegrationEvents.Services;
using BasketGrpc;
using EventBus;
using EventBus.Events.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

namespace Basket.API
{
    public class Startup
    {
        private readonly bool IS_ORCHESTRATED;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IS_ORCHESTRATED = Configuration.GetValue<bool>("IS_ORCHESTRATED");
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

            if (IS_ORCHESTRATED)
                services.AddCache(Cache.Enum.CachingServiceEnum.Redis);
            else
                services.AddCache(Cache.Enum.CachingServiceEnum.InMemory);

            services.AddGrpc();

            services.AddTransient<IBasketRepository, RedisBasketRepository>();

            services.AddTransient<IBasketIntegrationService, BasketIntegrationService>();

            services
                .AddEventBusRabbitMQ(Configuration)
                .AddCustomSwagger(Configuration)
                .ConfigureAuthService(Configuration);

            var hcBuilder = services.AddHealthChecks();

            hcBuilder
                .AddCheck("self", () => HealthCheckResult.Healthy());

            if (IS_ORCHESTRATED)
            {
                hcBuilder
                    .AddRedis(
                        Configuration["Redis:ConnectionService"],
                        name: "basket-redis-check",
                        tags: new string[] { "redis" });
            }


            hcBuilder
                   .AddRabbitMQ(
                       $"amqp://{Configuration["RabbitMQ:EventBusConnection"]}",
                       name: "basket-rabbitmqbus-check",
                       tags: new string[] { "rabbitmqbus" });
        }

        //Configure Autofac Container
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //Add IntegrationEventHandlers to Container
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
                app.UseSwaggerUI(setup =>
                {
                    setup.SwaggerEndpoint($"/swagger/v1/swagger.json", "Basket.API V1");
                    setup.OAuthClientId("basketswaggerui");
                    setup.OAuthAppName("Basket Swagger UI");
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
                    using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
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
                endpoints.MapGrpcService<BasketService>();
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

                var subscribeMethod = typeof(IEventBus)
                    .GetMethod("Subscribe")
                    .MakeGenericMethod(@event, handler.GetType());

                subscribeMethod.Invoke(eventBus, new object[] { @event.Name, "BasketApi", handler });
            }
        }

        internal static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "The Basket API",
                    Version = "v1",
                    Description = "The Basket API"
                });

                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrl")}/connect/authorize"),
                            TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrl")}/connect/token"),
                            Scopes = new Dictionary<string, string>()
                            {
                                { "basketApi.all", "Basket API" }
                            }
                        }
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
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
                options.Audience = "basketApi";
            });

            return services;
        }
    }
}
