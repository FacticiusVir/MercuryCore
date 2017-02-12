using Keeper.DotMudCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LoginExtensions
    {
        public static IServerBuilder UseLogin(this IServerBuilder builder)
        {
            return builder.UseMiddleware<LoginMiddleware>();
        }

        public static IServiceCollection AddSimpleLogin(this IServiceCollection services)
        {
            return services.AddSingleton<ILoginManager, SimpleLoginManager>();
        }
    }
}
