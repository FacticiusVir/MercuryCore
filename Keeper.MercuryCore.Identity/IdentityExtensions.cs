using Keeper.MercuryCore.Identity;
using Keeper.MercuryCore;
using Keeper.MercuryCore.Pipeline;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryIdentity(this IServiceCollection services)
        {
            services.AddSingleton<IUserManager, InMemoryUserManager>();

            return services;
        }

        public static IServiceCollection<IPipeline> UseSimpleLogin(this IServiceCollection<IPipeline> services)
        {
            services.AddSingleton<SimpleLoginManager>();
            services.Use<IdentityMiddleware<SimpleLoginManager>>();

            return services;
        }
    }
}
