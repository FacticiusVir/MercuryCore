using System;
using System.Threading;

namespace Keeper.MercuryCore
{
    public static class HostExtensions
    {
        public static void Run(this IHost host)
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (x, y) => tokenSource.Cancel();
                
                var hostTask = host.RunAsync(tokenSource.Token);

                Console.ReadLine();

                tokenSource.Cancel();

                hostTask.Wait();
            }
        }
    }
}
