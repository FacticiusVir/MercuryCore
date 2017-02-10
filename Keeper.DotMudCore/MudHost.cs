using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class MudHost
    {
        private IServiceCollection services;
        private IStartup startup;

        public MudHost(ServiceCollection services,
                        IStartup startup)
        {
            this.services = services;
            this.startup = startup;
        }

        public void Run()
        {
            using (var tokenSource = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (x, y) => tokenSource.Cancel();

                Task.Run(() =>
                {
                    Console.ReadLine();

                    tokenSource.Cancel();
                });

                this.Run(tokenSource.Token);
            }
        }

        public void Run(CancellationToken token)
        {
            var cancellationHandle = new ManualResetEventSlim(false);

            token.Register(() => cancellationHandle.Set());

            this.startup.ConfigureServices(this.services);

            var serverBuilder = new ServerBuilder();

            services.AddSingleton<IServerBuilder>(serverBuilder);

            var serviceProvider = this.services.BuildServiceProvider();

            serverBuilder.Services = serviceProvider;

            this.startup.Configure(serverBuilder);

            var server = ActivatorUtilities.CreateInstance<Server>(serviceProvider);

            server.Startup();

            cancellationHandle.Wait();

            server.Shutdown();
        }
    }
}