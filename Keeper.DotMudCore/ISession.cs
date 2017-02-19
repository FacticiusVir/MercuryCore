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
        public async static Task SendAsync(this ISession session, string message)
        {
            await EnsureActiveProtocol(session);

            await session.Protocol.Active.SendAsync(message);
        }

        public static Task SendLineAsync(this ISession session, string message = "")
        {
            return session.SendAsync(message + "\r\n");
        }

        public async static Task<string> ReceiveLineAsync(this ISession session)
        {
            await EnsureActiveProtocol(session);

            return await session.Protocol.Active.ReceiveLineAsync();
        }

        private async static Task EnsureActiveProtocol(ISession session)
        {
            if (session.Protocol.Active == null)
            {
                await session.Protocol.Get<IPlainAscii>().MakeActiveAsync();
            }
        }
    }
}
