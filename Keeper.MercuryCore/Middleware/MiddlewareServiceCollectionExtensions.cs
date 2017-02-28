using Keeper.MercuryCore;
using Keeper.MercuryCore.Middleware;
using Keeper.MercuryCore.Pipeline;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MiddlewareServiceCollectionExtensions
    {
        public static IServiceCollection<IPipeline> UseMotd(this IServiceCollection<IPipeline> services, Action<MotdOptions> optionsAction)
        {
            services.Configure(optionsAction);

            services.Use<MotdMiddleware>();

            return services;
        }
    }
}
