using Keeper.DotMudCore.Protocols.Internal;

namespace Keeper.DotMudCore
{
    public static class ProtocolServerBuilderExtensions
    {
        public static IServerBuilder UseLinemodeTelnet(this IServerBuilder server) => server.UseMiddleware<LinemodeTelnetMiddleware>();
    }
}
