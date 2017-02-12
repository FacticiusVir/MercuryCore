using Microsoft.Extensions.DependencyInjection;

namespace Keeper.DotMudCore
{
    public static class TcpExtensions
    {
        public static IServiceCollection AddTcpEndpoint(this IServiceCollection services)
        {
            return services.AddSingleton<IEndpoint, TcpEndpoint>();
        }
    }
}
