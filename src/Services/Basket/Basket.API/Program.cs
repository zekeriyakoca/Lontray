using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Basket.API
{
    public class Program
    {
        public static string Namespace => typeof(Startup).Namespace;
        public static string AppName => Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Run();
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
                    (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
                    {
                        var grpcPort = config.GetValue("GRPC_PORT", 81);
                        var port = config.GetValue("PORT", 80);
                        return (port, grpcPort);
                    }
                    webBuilder.UseStartup<Startup>();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseCustomSerilog(AppName);
    }
}
