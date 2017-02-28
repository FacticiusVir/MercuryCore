using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Session;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SessionServiceCollectionExtensions
    {
        public static IServiceCollection<IPipeline> UseAsciiChannel(this IServiceCollection<IPipeline> services)
        {
            object channelCreateLock = new object();
            AsciiChannel channel = null;

            Func<IServiceProvider, AsciiChannel> channelCreate = provider =>
            {
                lock (channelCreateLock)
                {
                    return channel ?? (channel = ActivatorUtilities.CreateInstance<AsciiChannel>(provider));
                }
            };

            services.AddSingleton<IChannel>(channelCreate);
            services.AddSingleton<ITextChannel>(channelCreate);

            return services;
        }
    }
}
