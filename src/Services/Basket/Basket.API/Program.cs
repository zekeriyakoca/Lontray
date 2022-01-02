using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        if (config.Build().GetValue<string>("IS_CONTAINER") == "true")
                            config.AddJsonFile("appsettings.Container.json", optional: false, reloadOnChange: false);
                    });
                    webBuilder.UseStartup<Startup>();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseCustomSerilog(AppName);
    }
}
