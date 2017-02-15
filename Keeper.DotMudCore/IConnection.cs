using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface IConnection
    {
        Task SendLineAsync(string message);

        Task<string> ReceiveLineAsync();

        void Close();

        string UniqueIdentifier { get; }
    }
}
