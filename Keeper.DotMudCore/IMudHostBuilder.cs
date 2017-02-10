using System;
using Microsoft.Extensions.DependencyInjection;

namespace Keeper.DotMudCore
{
    public interface IMudHostBuilder
    {
        MudHost Build();

        IMudHostBuilder ConfigureServices(Action<IServiceCollection> configureServices);
    }
}