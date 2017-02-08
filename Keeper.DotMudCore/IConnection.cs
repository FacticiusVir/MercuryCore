using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface IConnection
    {
        Task SendAsync(string message);

        Task<string> ReceiveAsync();

        void Close();

        string UniqueIdentifier { get; }
    }
}
