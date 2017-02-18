using Keeper.DotMudCore;
using System;

namespace Keeper.DotMudCore
{
    public static class MiddlewareServerBuilderExtensions
    {
        public static IServerBuilder UseMotd(this IServerBuilder server) => server.UseMiddleware<MotdMiddleware>();
    }
}

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MiddlewareServiceCollectionExtensions
    {
        public static IServiceCollection AddMotd(this IServiceCollection services, Action<MotdOptions> optionsAction) => services.Configure(optionsAction);
    }
}
