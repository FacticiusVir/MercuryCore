using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session
{
    public interface ITelnetChannel
        : ITextChannel
    {
        Task SendCommandAsync(TelnetCommand command, TelnetOption option);
    }
}
