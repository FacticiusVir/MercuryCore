using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Tcp;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TcpPipelineExtensions
    {
        public static IServiceCollection<IPipeline> AddTcpEndpoint(this IServiceCollection<IPipeline> services)
        {
            services.AddSingleton<IEndpoint, TcpEndpoint>();

            return services;
        }

        public static IServiceCollection<IPipeline> AddTcpEndpoint(this IServiceCollection<IPipeline> services, Action<TcpOptions> optionsAction)
        {
            services.Configure(optionsAction);

            return services.AddTcpEndpoint();
        }
    }
}
