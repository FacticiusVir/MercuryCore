using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface IConnection
    {
        Task SendAsync(string message);

        Task<string> ReceiveLineAsync();

        void Close();

        string UniqueIdentifier { get; }
    }

    public static class ConnectionExtensions
    {
        public static Task SendLineAsync(this IConnection connection, string message)
        {
            return connection.SendAsync(message + "\n\r");
        }
    }
}
