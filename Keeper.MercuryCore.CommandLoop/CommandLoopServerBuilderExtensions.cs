using Keeper.MercuryCore.CommandLoop.Internal;
using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace Keeper.MercuryCore
{
    public static class CommandLoopServerBuilderExtensions
    {
        public static IServiceCollection<IPipeline> UseCommandLoop(this IServiceCollection<IPipeline> services)
        {
            return services.Use<CommandLoopMiddleware>();
        }
    }
}
