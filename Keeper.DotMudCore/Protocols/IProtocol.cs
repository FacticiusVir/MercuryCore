using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols
{
    public interface IProtocol
    {
        Task SendAsync(string message);

        Task<string> ReceiveLineAsync();
    }
}
