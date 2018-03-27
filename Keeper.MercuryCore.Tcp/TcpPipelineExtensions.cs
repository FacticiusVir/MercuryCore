using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Tcp;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TcpPipelineExtensions
    {
        public static IServiceCollection<IPipeline> AddTcpEndpoint(this IServiceCollection<IPipeline> services, string name)
        {
            services.AddSingleton<IEndpoint>(provider => ActivatorUtilities.CreateInstance<TcpEndpoint>(provider, name));

            return services;
        }

        public static IServiceCollection<IPipeline> AddTcpEndpoint(this IServiceCollection<IPipeline> services, string name, Action<TcpOptions> optionsAction)
        {
            services.Configure(name, optionsAction);

            return services.AddTcpEndpoint(name);
        }
    }
}
