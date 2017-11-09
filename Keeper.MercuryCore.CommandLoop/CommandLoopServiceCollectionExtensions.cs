using Keeper.MercuryCore;
using Keeper.MercuryCore.CommandLoop;
using Keeper.MercuryCore.CommandLoop.Internal;
using Keeper.MercuryCore.Util;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommandLoopServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandLoop(this IServiceCollection services, Action<IServiceCollection<ICommandLoop>> servicesAction)
        {
            var collectionWrapper = new WrappedServiceCollection<ICommandLoop>(services);

            servicesAction(collectionWrapper);

            return services;
        }

        public static IServiceCollection<ICommandLoop> AddQuitHandler(this IServiceCollection<ICommandLoop> services) => services.AddHandler<QuitCommandHandler>();

        public static IServiceCollection<ICommandLoop> AddHandler<T>(this IServiceCollection<ICommandLoop> services)
            where T : class, ICommandHandler
        {
            services.AddSingleton<T>();
            services.AddSingleton<ICommandHandler>(provider => provider.GetRequiredService<T>());

            return services;
        }
    }
}
