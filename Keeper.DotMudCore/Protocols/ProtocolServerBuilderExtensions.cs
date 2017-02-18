using Keeper.DotMudCore.Protocols.Internal;

namespace Keeper.DotMudCore
{
    public static class ProtocolServerBuilderExtensions
    {
        public static IServerBuilder UseTelnet(this IServerBuilder server) => server.UseMiddleware<TelnetMiddleware>();
    }
}
