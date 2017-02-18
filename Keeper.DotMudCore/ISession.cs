using Keeper.DotMudCore.Protocols;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface ISession
    {
        IConnection Connection { get; }

        ISessionStateManager State { get; }

        IProtocolManager Protocol { get; }
    }

    public static class SessionExtensions
    {
        public static Task SendAsync(this ISession session, string message)
        {
            return session.Protocol.Active.SendAsync(message);
        }

        public static Task SendLineAsync(this ISession session, string message = "")
        {
            return session.SendAsync(message + "\r\n");
        }

        public static Task<string> ReceiveLineAsync(this ISession session)
        {
            return session.Protocol.Active.ReceiveLineAsync();
        }
    }
}
