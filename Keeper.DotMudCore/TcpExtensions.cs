using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
