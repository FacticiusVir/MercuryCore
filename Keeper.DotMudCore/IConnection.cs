using System.Threading;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface IConnection
    {
        Task SendAsync(byte[] data, int offset, int count);

        Task<int> ReceiveAsync(byte[] data, int offset, int count);

        Task<int> ReceiveAsync(byte[] data, int offset, int count, CancellationToken token);

        void Close();

        string UniqueIdentifier { get; }
    }

    public static class ConnectionExtensions
    {
        public static Task SendAsync(this IConnection connection, byte[] data, int offset = 0)
        {
            return connection.SendAsync(data, offset, data.Length - offset);
        }

        public static Task<int> ReceiveAsync(this IConnection connection, byte[] data, int offset = 0)
        {
            return connection.ReceiveAsync(data, offset, data.Length - offset);
        }
    }
}
