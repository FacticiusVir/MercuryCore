using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Keeper.MercuryCore.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                            .ConfigureSerilog(config => config
                                                            .WriteTo.LiterateConsole()
                                                            .MinimumLevel.Debug())
                            .ConfigurePipeline(pipeline =>
                            {
                                pipeline.AddTcpEndpoint(options => options.Port = 5000);

                                pipeline.UseAsciiChannel();

                                pipeline.UseMotd(options => options.Message = "Welcome to the Test Server!");

                                pipeline.Use((provider, next) =>
                                {
                                    var channel = provider.GetRequiredService<Session.ITextChannel>();

                                    return async () =>
                                    {
                                        await channel.ReceiveLineAsync();

                                        await next();
                                    };
                                });
                            })
                            .Build();

            host.Run();
        }
    }
}