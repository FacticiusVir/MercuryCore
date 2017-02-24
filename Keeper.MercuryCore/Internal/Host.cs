using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Internal
{
    internal class Host
        : IHost
    {
        private IServiceCollection hostServices;

        public Host(IServiceCollection hostServices)
        {
            this.hostServices = hostServices;
        }

        public Task RunAsync(CancellationToken token)
        {
            return Task.Run(() =>
            {
                var cancellationHandle = new ManualResetEventSlim(false);

                token.Register(() => cancellationHandle.Set());

                var hostServiceProvider = this.hostServices.BuildServiceProvider();
                
                foreach (var startup in hostServiceProvider.GetServices<IHostStartup>())
                {
                    startup.Run(hostServiceProvider);
                }

                var pipelines = hostServiceProvider.GetServices<IPipelineFactory>()
                                                    .Select((factory, index) => factory.Create(hostServiceProvider, index)).ToList();
                
                foreach (var pipeline in pipelines)
                {
                    pipeline.Start();
                }

                cancellationHandle.Wait();

                foreach (var pipeline in pipelines)
                {
                    pipeline.Stop();
                }
            });
        }
    }
}
