using Keeper.DotMudCore.Identity;

namespace Keeper.DotMudCore
{
    public static class LoginServerBuilderExtensions
    {
        public static IServerBuilder UseLogin(this IServerBuilder builder)
        {
            return builder.UseMiddleware<LoginMiddleware>();
        }
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoginServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleLogin(this IServiceCollection services)
        {
            return services.AddSingleton<IUserManager, InMemoryUserManager>()
                            .AddSingleton<ILoginManager, SimpleLoginManager>();
        }
    }
}
