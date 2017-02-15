using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.DotMudCore.HelloWorldSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new MudHostBuilder()
                            .ConfigureServices(services => services.AddTcpEndpoint(options => options.Port = 5000))
                            .Configure(server => server.Run(async session =>
                            {
                                await session.Connection.SendLineAsync("Hello World!");
                                await session.Connection.ReceiveLineAsync();
                            }))
                            .Build();

            Console.WriteLine("Running");

            host.Run();
        }
    }
}