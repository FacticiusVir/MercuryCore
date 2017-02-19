using Keeper.DotMudCore;
using System;

namespace Keeper.SimpleMud
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