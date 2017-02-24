using Keeper.MercuryCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Keeper.MercuryCore
{
    public class HostBuilder
        : IHostBuilder
    {
        private List<Action<IServiceCollection>> serviceConfigurations = new List<Action<IServiceCollection>>();

        public IHostBuilder ConfigureServices(Action<IServiceCollection> servicesAction)
        {
            this.serviceConfigurations.Add(servicesAction);

            return this;
        }

        public IHost Build()
        {
            var hostServices = new ServiceCollection();

            foreach (var serviceAction in this.serviceConfigurations)
            {
                serviceAction(hostServices);
            }

            hostServices.AddLogging();
            hostServices.AddOptions();

            return new Host(hostServices);
        }
    }
}
