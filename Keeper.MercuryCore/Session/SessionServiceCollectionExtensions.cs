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
        public static IServiceCollection<IPipeline> UseAsciiChannel(this IServiceCollection<IPipeline> services) => services.UsePlainTextChannel(Encoding.ASCII);

        public static IServiceCollection<IPipeline> UseUtf8Channel(this IServiceCollection<IPipeline> services) => services.UsePlainTextChannel(Encoding.UTF8);

        public static IServiceCollection<IPipeline> UsePlainTextChannel(this IServiceCollection<IPipeline> services, Encoding textEncoding)
        {
            services.AddScoped(provider => ActivatorUtilities.CreateInstance<PlainTextChannel>(provider, textEncoding));
            services.AddScoped<IChannel>(provider => provider.GetService<PlainTextChannel>());
            services.AddScoped<ITextChannel>(provider => provider.GetService<PlainTextChannel>());

            return services;
        }

        public static IServiceCollection<IPipeline> UseVirtualTerminalChannel(this IServiceCollection<IPipeline> services)
        {
            services.AddScoped<VirtualTerminalChannel>();
            services.AddScoped<IChannel>(provider => provider.GetService<VirtualTerminalChannel>());
            services.AddScoped<IVirtualTerminalChannel>(provider => provider.GetService<VirtualTerminalChannel>());

            return services;
        }

        public static IServiceCollection<IPipeline> UseTelnetChannel(this IServiceCollection<IPipeline> services)
        {
            services.AddScoped(provider => ActivatorUtilities.CreateInstance<TelnetChannel>(provider));
            services.AddScoped<IChannel>(provider => provider.GetService<TelnetChannel>());
            services.AddScoped<ITelnetChannel>(provider => provider.GetService<TelnetChannel>());

            return services;
        }
    }
}
