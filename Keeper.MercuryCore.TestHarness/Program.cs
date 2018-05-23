using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Text;
using System.Threading.Tasks;

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
                                        pipeline.AddTcpEndpoint("tcp", options => options.Port = 1234);
                                        pipeline.UseTelnetChannel(Encoding.UTF8);

                                        pipeline.Use((provider, next) => () => Run(provider));
                                    })
                                    .Build();

            host.Run();
        }

        private static async Task Run(IServiceProvider provider)
        {
            var channel = provider.GetRequiredService<ITelnetChannel>();

            await channel.SendLineAsync("Sandbox");

            while (await channel.ReceiveLineAsync() != "quit") ;
        }
    }
}