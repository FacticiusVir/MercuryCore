using Keeper.DotMudCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class TcpExtensions
    {
        public static IServiceCollection AddTcpEndpoint(this IServiceCollection services) => services.AddSingleton<IEndpoint, TcpEndpoint>();

        public static IServiceCollection AddTcpEndpoint(this IServiceCollection services, Action<TcpOptions> optionsAction) => services.Configure(optionsAction).AddSingleton<IEndpoint, TcpEndpoint>();
    }
}
