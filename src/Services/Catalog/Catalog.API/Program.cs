using Autofac.Extensions.DependencyInjection;
using Catalog.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Microsoft.Extensions.Configuration;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Catalog.API
{
    public class Program
    {
        public static string Namespace => typeof(Startup).Namespace;
        public static string AppName => Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                host.Migrate<CatalogContext>(seederAction: (context, services) =>
                {
                    context.Database.EnsureCreated();
                    services.GetService<CatalogContextSeeder>().Seed().Wait();
                });

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    IConfiguration configuration = default;
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        if (config.Build().GetValue<string>("IS_CONTAINER") == "true")
                            config.AddJsonFile("appsettings.Container.json", optional: false, reloadOnChange: false);
                        configuration = config.Build();
                    });
                    webBuilder.ConfigureKestrel(options =>
                     {
                         var ports = GetDefinedPorts(configuration);
                         options.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                         {
                             listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                         });
                         options.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                         {
                             listenOptions.Protocols = HttpProtocols.Http2;
                         });

                     });
                    webBuilder.UseStartup<Startup>();

                    (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
                    {
                        var grpcPort = config.GetValue("GRPC_PORT", 81);
                        var port = config.GetValue("PORT", 80);
                        return (port, grpcPort);
                    }
                })

                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseCustomSerilog(AppName);


    }

}
