using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Keeper.MercuryCore.Util
{
    public class ChildServiceCollection<T>
        : List<ServiceDescriptor>, IServiceCollection<T>, IDisposable
    {
        private IServiceScope parentScope;

        public ChildServiceCollection(IEnumerable<ServiceDescriptor> parent, IServiceProvider parentProvider)
        {
            this.parentScope = parentProvider.CreateScope();

            foreach (var service in parent)
            {
                if (service.ServiceType.GetTypeInfo().IsGenericType)
                {
                    if (service.Lifetime == ServiceLifetime.Transient)
                    {
                        this.AddTransient(service.ServiceType, service.ImplementationType);
                    }
                    else
                    {
                        if (service.ImplementationInstance != null)
                        {
                            this.AddSingleton(service.ServiceType, service.ImplementationInstance);
                        }
                        else
                        {
                            this.AddSingleton(service.ServiceType, service.ImplementationType);
                        }
                    }
                }
                else
                {
                    if (service.Lifetime == ServiceLifetime.Transient)
                    {
                        this.AddTransient(service.ServiceType, x => this.parentScope.ServiceProvider.GetService(service.ServiceType));
                    }
                    else
                    {
                        if (service.ImplementationFactory != null)
                        {
                            this.AddSingleton(service.ServiceType, service.ImplementationFactory);
                        }
                        else if (service.ImplementationInstance != null)
                        {
                            this.AddSingleton(service.ServiceType, service.ImplementationInstance);
                        }
                        else
                        {
                            this.AddSingleton(service.ServiceType, service.ImplementationType);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            this.parentScope.Dispose();
        }
    }
}
