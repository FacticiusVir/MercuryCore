using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.MercuryCore
{
    public interface IHostBuilder
    {
        IHostBuilder ConfigureServices(Action<IServiceCollection> servicesAction);

        IHostBuilder ConfigurePipeline(Action<IServiceCollection<IPipeline>> pipelineAction);

        IHost Build();
    }
}
