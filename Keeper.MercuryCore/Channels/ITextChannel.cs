using System.Threading.Tasks;

namespace Keeper.MercuryCore.Channels
{
    public interface ITextChannel
    {
        Task SendAsync(string message);
    }
}
