using Keeper.DotMudCore.Protocols;

namespace Keeper.DotMudCore
{
    public static class ProtocolSessionExtensions
    {
        public static bool IsLinemodeTelnetSupported(this ISession session)
        {
            return session.Protocol.GetSupport<ILinemodeTelnet>() == ProtocolSupport.Supported;
        }

        public static ILinemodeTelnet GetLinemodeTelnet(this ISession session)
        {
            return session.Protocol.Get<ILinemodeTelnet>();
        }
    }
}
