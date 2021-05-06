using Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Extensions.Hosting
{
    public static class SerilogLoggingHelper
    {
        public static IHostBuilder UseCustomSerilog(this IHostBuilder hostBuilder, string AppName)
        {
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                var settings = hostingContext.Configuration.GetSection("Logging").Get<LoggingSettings>();

                var elasticBufferRootName = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "Logs", settings.ElasticBufferRoot);
                var roolingFileName = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "Logs", settings.RoolingFileName);

                loggerConfiguration
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", AppName)
                .ReadFrom.Configuration(hostingContext.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(roolingFileName);
                if (settings.IsSeqActive)
                    loggerConfiguration.WriteTo.Seq(settings.SeqServerUrl);
                if (settings.IsElkActive)
                {
                    var elasticSinkOptions = new Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions(new Uri(settings.ElasticSearchUrl))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv6,
                        IndexFormat = settings.ElasticIndexFormatRoot + "-{0:yyyy.MM.dd}",
                        BufferBaseFilename = elasticBufferRootName,
                        InlineFields = true,
                        MinimumLogEventLevel = Serilog.Events.LogEventLevel.Warning // Set min eventlevel to Warning for Elk
                    };
                    if (settings.HasElkCredentials)
                    {
                        elasticSinkOptions.ModifyConnectionSettings = x => x.BasicAuthentication(settings.ElasticSearchUsername, settings.ElasticSearchPassword);
                    }

                    loggerConfiguration.WriteTo.Elasticsearch(elasticSinkOptions);
                }
            });

            return hostBuilder;
        }
    }
}
