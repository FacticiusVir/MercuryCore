using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Session;
using Keeper.MercuryCore.Session.Internal;
using System;
using System.Text;

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

        public static IServiceCollection<IPipeline> UseTelnetChannel(this IServiceCollection<IPipeline> services, Encoding encoding = null)
        {
            object channelCreateLock = new object();
            TelnetChannel channel = null;

            Func<IServiceProvider, TelnetChannel> channelCreate = provider =>
            {
                lock (channelCreateLock)
                {
                    return channel ?? (channel = ActivatorUtilities.CreateInstance<TelnetChannel>(provider, encoding ?? Encoding.ASCII));
                }
            };

            services.AddSingleton<IChannel>(channelCreate);
            services.AddSingleton<ITextChannel>(channelCreate);

            return services;
        }
    }
}
