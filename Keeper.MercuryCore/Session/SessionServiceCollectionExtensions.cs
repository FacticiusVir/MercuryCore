using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Session;
using Keeper.MercuryCore.Session.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SessionServiceCollectionExtensions
    {
        public static IServiceCollection<IPipeline> UseAsciiChannel(this IServiceCollection<IPipeline> services)
        {
            services.AddScoped<AsciiChannel>();
            services.AddScoped<IChannel>(provider => provider.GetService<AsciiChannel>());
            services.AddScoped<ITextChannel>(provider => provider.GetService<AsciiChannel>());

            return services;
        }

        public static IServiceCollection<IPipeline> UseTelnetChannel(this IServiceCollection<IPipeline> services, Encoding encoding = null)
        {
            services.AddScoped(provider => ActivatorUtilities.CreateInstance<TelnetChannel>(provider, encoding ?? Encoding.ASCII));
            services.AddScoped<IChannel>(provider => provider.GetService<TelnetChannel>());
            services.AddScoped<ITextChannel>(provider => provider.GetService<TelnetChannel>());

            return services;
        }
    }
}
