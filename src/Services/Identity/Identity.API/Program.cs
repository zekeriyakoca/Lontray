using Autofac.Extensions.DependencyInjection;
using IdentityServer4.EntityFramework.DbContexts;
using Lontray.Services.Identity.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Lontray.Services.Identity.API
{
    public class Program
    {
        static string Namespace => typeof(Startup).Namespace;
        static string AppName => Namespace.Substring(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1);

        public static void Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();

                host.Migrate<PersistedGrantDbContext>();
                host.Migrate<ApplicationDbContext>(seederAction: (context, services) =>
                {
                    var env = services.GetService<IWebHostEnvironment>();
                    var logger = services.GetService<ILogger<Program>>();
                    new ApplicationDbContextSeeder().SeedAsync((ApplicationDbContext)context, env, logger).Wait();
                });
                host.Migrate<ConfigurationDbContext>(seederAction: (context, services) =>
                {
                    var configuration = services.GetRequiredService<IConfiguration>();

                    new ConfigurationDbContextSeeder()
                       .SeedAsync((ConfigurationDbContext)context, configuration)
                       .Wait();
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
