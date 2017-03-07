using Keeper.MercuryCore.Session;
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

                                pipeline.UseTelnetChannel();

                                pipeline.UseMotd(options => options.Message = "Welcome to the Test Server!");

                                pipeline.UseSimpleLogin();

                                pipeline.Use((provider, next) =>
                                {
                                    var channel = provider.GetRequiredService<ITextChannel>();
                                    var sessionState = provider.GetRequiredService<IStateManager>();

                                    return async () =>
                                    {
                                        var identityInfo = sessionState.GetIdentityInfo();

                                        await channel.SendLineAsync($"Hi {identityInfo.Username}!");

                                        await channel.ReceiveLineAsync();
                                    };
                                });
                            })
                            .Build();

            host.Run();
        }
    }
}