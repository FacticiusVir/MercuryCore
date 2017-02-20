using Keeper.DotMudCore.CommandLoop.Internal;

namespace Keeper.DotMudCore
{
    public static class CommandLoopServerBuilderExtensions
    {
        public static IServerBuilder UseCommandLoop(this IServerBuilder server)
        {
            return server.UseMiddleware<CommandLoopMiddleware>();
        }
    }
}
