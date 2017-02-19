using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace Keeper.DotMudCore
{
    public static class SerilogServerBuilderExtensions
    {
        public static IServerBuilder UseSerilog(this IServerBuilder server)
        {
            server.Services.GetService<ILoggerFactory>().AddSerilog();

            return server;
        }

        public static IServerBuilder UseSerilog(this IServerBuilder server, Serilog.ILogger logger)
        {
            server.Services.GetService<ILoggerFactory>().AddSerilog(logger);

            return server;
        }

        public static IServerBuilder UseSerilog(this IServerBuilder server, Action<LoggerConfiguration> loggerAction)
        {
            var configuration = new LoggerConfiguration();

            loggerAction(configuration);

            server.Services.GetService<ILoggerFactory>().AddSerilog(configuration.CreateLogger());

            return server;
        }
    }
}
