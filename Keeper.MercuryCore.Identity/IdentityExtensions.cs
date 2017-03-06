using Keeper.MercuryCore.Identity;
using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection<IPipeline> UseSimpleLogin(this IServiceCollection<IPipeline> services)
        {
            services.AddSingleton<IUserManager, InMemoryUserManager>();
            services.AddSingleton<IIdentityManager, SimpleLoginManager>();
            services.Use<IdentityMiddleware>();

            return services;
        }
    }
}
