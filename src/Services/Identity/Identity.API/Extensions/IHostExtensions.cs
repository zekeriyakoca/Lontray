using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    public static class IHostExtensions
    {
        public static IHost Migrate<TContext>(this IHost host, Action<DbContext, IServiceProvider> seederAction) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<IHost>>();
                var context = services.GetRequiredService<TContext>();

                var retries = 10;
                var policy = Policy.Handle<SqlException>()
                    .WaitAndRetry(retryCount: retries,
                    (retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", nameof(TContext), exception.GetType().Name, exception.Message, retry, retries);

                    });
                try
                {
                    policy.Execute(() =>
                    {
                        context.Database.Migrate();
                        seederAction.Invoke(context, services);
                    });
                    logger.LogInformation("{DbContext} migrated succesfully");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occured while migrating&seeding database for {DbContext}", nameof(TContext));
                }
            }

            return host;

        }
        public static IHost Migrate<TContext>(this IHost host) where TContext : DbContext
        {
            return host.Migrate<TContext>((_, _) => { });
        }
    }
}
