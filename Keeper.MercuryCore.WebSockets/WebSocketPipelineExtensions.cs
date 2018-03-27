using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.WebSockets;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class WebSocketPipelineExtensions
    {
        public static IServiceCollection<IPipeline> AddWebSocketEndpoint(this IServiceCollection<IPipeline> services, string name)
        {
            services.AddSingleton<IEndpoint>(provider => ActivatorUtilities.CreateInstance<WebSocketEndpoint>(provider, name));

            return services;
        }

        //public static IServiceCollection<IPipeline> AddTcpEndpoint(this IServiceCollection<IPipeline> services, Action<TcpOptions> optionsAction)
        //{
        //    services.Configure(optionsAction);

        //    return services.AddTcpEndpoint();
        //}
    }
}
