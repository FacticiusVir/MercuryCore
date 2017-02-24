using Keeper.MercuryCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Keeper.MercuryCore
{
    public static class SerilogServerBuilderExtensions
    {
        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
            => builder.ConfigureStartup(services => services.GetService<ILoggerFactory>().AddSerilog());

        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder, Serilog.ILogger logger)
            => builder.ConfigureStartup(services => services.GetService<ILoggerFactory>().AddSerilog(logger));

        public static IHostBuilder ConfigureSerilog(this IHostBuilder builder, Action<LoggerConfiguration> loggerAction)
        {
            return builder.ConfigureStartup(services =>
            {
                var configuration = new LoggerConfiguration();

                loggerAction(configuration);

                services.GetService<ILoggerFactory>().AddSerilog(configuration.CreateLogger());
            });
        }
    }
}
