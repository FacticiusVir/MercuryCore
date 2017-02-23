using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Keeper.MercuryCore.Internal
{
    internal class ChildServiceCollection
        : List<ServiceDescriptor>, IServiceCollection, IDisposable
    {
        private IServiceScope parentScope;

        public ChildServiceCollection(IServiceCollection parent, IServiceProvider parentProvider)
        {
            this.parentScope = parentProvider.CreateScope();

            foreach (var service in parent)
            {
                if (service.Lifetime == ServiceLifetime.Transient)
                {
                    this.AddTransient(service.ServiceType, x => this.parentScope.ServiceProvider.GetService(service.ServiceType));
                }
                else
                {
                    this.AddSingleton(service.ServiceType, x => this.parentScope.ServiceProvider.GetService(service.ServiceType));
                }
            }
        }

        public void Dispose()
        {
            this.parentScope.Dispose();
        }
    }
}
