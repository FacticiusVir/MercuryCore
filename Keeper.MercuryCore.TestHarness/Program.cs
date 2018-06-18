using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                                    .ConfigureServices(services => services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace)))
                                    .ConfigureSerilog(config => config
                                                                    .WriteTo.LiterateConsole()
                                                                    .WriteTo.RollingFile(".\\Logs\\TestHarness-{Date}.log", fileSizeLimitBytes: 1024 * 1024)
                                                                    .MinimumLevel.Verbose())
                                    .ConfigurePipeline(pipeline =>
                                    {
                                        pipeline.AddTcpEndpoint("tcp", options => options.Port = 5000);
                                        pipeline.UseTelnetChannel();
                                        pipeline.UseVirtualTerminalChannel();
                                        pipeline.UseUtf8Channel();

                                        pipeline.Use((provider, next) => () => Run(provider));
                                    })
                                    .Build();

            host.Run();
        }

        private static async Task Run(IServiceProvider provider)
        {
            var channel = provider.GetRequiredService<ITextChannel>();
            var virtualTerminal = provider.GetRequiredService<IVirtualTerminalChannel>();

            await channel.SendLineAsync("Sandbox");

            await channel.SendLineAsync("Enter 'quit' to disconnect");

            string line = null;

            while (line != "quit")
            {
                line = await channel.ReceiveLineAsync();
            }
        }
    }
}