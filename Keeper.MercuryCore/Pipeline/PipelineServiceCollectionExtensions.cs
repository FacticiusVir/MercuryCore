using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Pipeline.Internal;
using System;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class PipelineServiceCollectionExtensions
    {
        public static IServiceCollection<IPipeline> BindEndpoint(this IServiceCollection<IPipeline> services, string name)
        {
            services.AddSingleton<IPipelineEndpointBinding>(new PipelineEndpointBinding(name));

            return services;
        }

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
