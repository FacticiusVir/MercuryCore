using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.MercuryCore
{
    public interface IHostBuilder
    {
        IHostBuilder ConfigureServices(Action<IServiceCollection> servicesAction);

        IHost Build();
    }
}
