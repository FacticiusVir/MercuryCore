using Keeper.DotMudCore.Identity;

namespace Keeper.DotMudCore
{
    public static class IdentityServerBuilderExtensions
    {
        public static IServerBuilder UseIdentity(this IServerBuilder builder)
        {
            return builder.UseMiddleware<IdentityMiddleware>();
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleLogin(this IServiceCollection services)
        {
            return services.AddSingleton<IUserManager, InMemoryUserManager>()
                            .AddSingleton<IIdentityManager, SimpleLoginManager>();
        }
    }
}
