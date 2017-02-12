using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Keeper.DotMudCore
{
    public class MudHostBuilder
        : IMudHostBuilder
    {
        private List<Action<IServiceCollection>> serviceConfigurations = new List<Action<IServiceCollection>>();

        public IMudHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            this.serviceConfigurations.Add(configureServices);

            return this;
        }

        public MudHost Build()
        {
            var services = new ServiceCollection();

            foreach(var serviceConfiguration in this.serviceConfigurations)
            {
                serviceConfiguration(services);
            }

            var hostingServices = services.BuildServiceProvider();

            services.AddLogging();
            services.AddOptions();

            return new MudHost(services, hostingServices.GetRequiredService<IStartup>());
        }
    }

    public static class MudHostBuilderExtensions
    {
        public static IMudHostBuilder Configure(this IMudHostBuilder builder, Action<IServerBuilder> configure)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartup>(new DelegateStartup(configure));
            });
        }

        public static IMudHostBuilder UseStartup<TStartup>(this IMudHostBuilder builder)
            where TStartup : class, IStartup, new()
        {
            return builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartup, TStartup>();
            });
        }
    }
}
