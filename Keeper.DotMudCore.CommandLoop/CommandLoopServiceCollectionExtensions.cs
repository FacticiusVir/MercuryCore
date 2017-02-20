﻿using Keeper.DotMudCore.CommandLoop;
using Keeper.DotMudCore.CommandLoop.Internal;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CommandLoopServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandLoop(this IServiceCollection services, Action<ICommandLoopServiceCollection> servicesAction)
        {
            var collectionWrapper = new CommandLoopServiceCollection(services);

            servicesAction(collectionWrapper);

            return services;
        }
    }
}
