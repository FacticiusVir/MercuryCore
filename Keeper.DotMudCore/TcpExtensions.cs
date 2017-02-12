using Keeper.DotMudCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TcpExtensions
    {
        public static IServiceCollection AddTcpEndpoint(this IServiceCollection services)
        {
            return services.AddSingleton<IEndpoint, TcpEndpoint>();
        }
        public static IServiceCollection AddTcpEndpoint(this IServiceCollection services, Action<TcpOptions> optionsAction)
        {
            return services
                        .Configure(optionsAction)
                        .AddSingleton<IEndpoint, TcpEndpoint>();
        }
    }
}
