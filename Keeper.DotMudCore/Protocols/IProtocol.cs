using System;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols
{
    public interface IProtocol
    {
        Task SendAsync(string message);

        Task<string> ReceiveLineAsync();

        Task MakeActiveAsync();

        IDisposable CreateActiveSession();
    }

    public static class ProtocolExtensions
    {
        public static Task SendLineAsync(this IProtocol protocol, string message = "")
        {
            return protocol.SendAsync(message + "\r\n");
        }
    }
}
