using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineServiceCollectionExtensions
    {
        public static IServiceCollection<IPipeline> Use<T>(this IServiceCollection<IPipeline> services)
            where T : class, IMiddleware
        {
            services.AddSingleton<IMiddleware, T>();

            return services;
        }
        public static IServiceCollection<IPipeline> Use(this IServiceCollection<IPipeline> services, Func<IServiceProvider, Func<Task>, Func<Task>> middleware)
        {
            services.AddSingleton<IMiddleware>(new DelegateMiddleware(middleware));

            return services;
        }
    }
}
