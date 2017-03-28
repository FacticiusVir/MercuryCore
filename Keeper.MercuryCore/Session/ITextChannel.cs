using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session
{
    public interface ITextChannel
        : IChannel
    {
        Task SendLineAsync(string message);

        Task<string> ReceiveLineAsync();
    }
}
