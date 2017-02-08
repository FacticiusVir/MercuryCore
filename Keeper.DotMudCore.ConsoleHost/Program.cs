using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using System;

namespace Keeper.DotMudCore.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = BuildServiceProvider();
            SetupSerilog(serviceProvider);

            var server = ActivatorUtilities.CreateInstance<Server>(serviceProvider);

            server.Startup();

            Console.ReadLine();

            server.Shutdown();

            Console.ReadLine();
        }

        private static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                            .AddLogging()
                            .AddSingleton<IEndpoint>(new TcpEndpoint(5000))
                            .AddSingleton<ILoginManager, SimpleLoginManager>()
                            .BuildServiceProvider();
        }

        private static void SetupSerilog(IServiceProvider serviceProvider)
        {
            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .WriteTo.ColoredConsole()
                            .WriteTo.File(new JsonFormatter(), ".\\log.txt")
#if DEBUG
                            .MinimumLevel.Debug()
#endif
                            .CreateLogger();

            serviceProvider.GetService<ILoggerFactory>().AddSerilog();
        }
    }
}