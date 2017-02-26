using Keeper.MercuryCore.Middleware;
using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.MercuryCore
{
    public static class MiddlewareServiceCollectionExtensions
    {
        public static IServiceCollection<IPipeline> UseMotd(this IServiceCollection<IPipeline> services, Action<MotdOptions> optionsAction)
        {
            services.Configure(optionsAction);

            services.AddSingleton<IMiddleware, MotdMiddleware>();

            return services;
        }
    }
}
