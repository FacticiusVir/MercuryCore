using System;
using System.Threading;
using System.Threading.Tasks;

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

                var consoleTask = Task.Run(() =>
                {
                    Console.ReadLine();

                    tokenSource.Cancel();
                });

                Task.WaitAny(hostTask, consoleTask);

                hostTask.Wait();
            }
        }
    }
}
