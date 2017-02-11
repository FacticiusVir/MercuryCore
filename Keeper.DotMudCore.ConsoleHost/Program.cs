using Microsoft.Extensions.DependencyInjection;

namespace Keeper.DotMudCore.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new MudHostBuilder()
                            .ConfigureServices(services =>
                            {
                                services.AddSingleton<IEndpoint, TcpEndpoint>();
                            })
                            .UseStartup<Startup>()
                            .Build();

            host.Run();
        }
    }
}