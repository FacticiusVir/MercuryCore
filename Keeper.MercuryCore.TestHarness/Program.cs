using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            channel.Negotiation.LinkTo(new ActionBlock<(TelnetCommand Command, TelnetOption Option)>(async negotiation =>
            {
                if (negotiation.Command == TelnetCommand.WILL)
                {
                    await channel.SendCommandAsync(TelnetCommand.DO, negotiation.Option);
                }
                else if (negotiation.Command == TelnetCommand.DO)
                {
                    //if (negotiation.Option == TelnetOption.SuppressGoAhead)
                    //{
                    //    channel.SendCommandAsync(TelnetCommand.WILL, negotiation.Option);
                    //}
                    //else
                    //{
                    await channel.SendCommandAsync(TelnetCommand.WONT, negotiation.Option);
                    //}
                }
            }));

            channel.SubNegotiation.LinkTo(new ActionBlock<(TelnetCommand Command, IReceivableSourceBlock<byte> Data)>(async subNegotiation =>
            {
                var data = new List<byte>();

                while (!subNegotiation.Data.Completion.IsCompleted)
                {
                    data.Add(await subNegotiation.Data.ReceiveAsync());
                }

                Console.WriteLine($"SubNegotiation {subNegotiation.Command} {string.Join(", ", data.Select(x => x.ToString("x2")))}");
            }));

            while (await channel.ReceiveLineAsync() != "quit") await channel.SendLineAsync("Enter 'quit' to disconnect");
        }
    }
}