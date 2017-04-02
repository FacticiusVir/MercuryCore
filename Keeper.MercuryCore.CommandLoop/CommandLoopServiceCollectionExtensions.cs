using Keeper.MercuryCore;
using Keeper.MercuryCore.CommandLoop;
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
    }
}
