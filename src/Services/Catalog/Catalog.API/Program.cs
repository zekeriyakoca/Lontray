using Autofac.Extensions.DependencyInjection;
using Catalog.API.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

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
                    //var env = services.GetService<IWebHostEnvironment>();
                    //var logger = services.GetService<ILogger<Program>>();
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
                    webBuilder.UseStartup<Startup>();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseCustomSerilog(AppName);
    }
}
