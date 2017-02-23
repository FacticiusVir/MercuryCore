using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Keeper.MercuryCore.Internal;

namespace Keeper.MercuryCore
{
    public class HostBuilder
        : IHostBuilder
    {
        private List<Action<IServiceCollection>> serviceConfigurations = new List<Action<IServiceCollection>>();

        public IHostBuilder ConfigurePipeline(Action<IServiceCollection<IPipeline>> pipelineAction)
        {
            throw new NotImplementedException();
        }

        public IHostBuilder ConfigureServices(Action<IServiceCollection> servicesAction)
        {
            throw new NotImplementedException();
        }

        public IHost Build()
        {
            return new Host();
        }
    }
}
