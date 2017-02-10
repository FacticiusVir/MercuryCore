using Microsoft.Extensions.DependencyInjection;

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
