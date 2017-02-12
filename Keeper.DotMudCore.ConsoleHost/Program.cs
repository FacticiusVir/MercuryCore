using Microsoft.Extensions.DependencyInjection;

namespace Keeper.DotMudCore.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new MudHostBuilder()
                            .UseStartup<Startup>()
                            .Build();

            host.Run();
        }
    }
}