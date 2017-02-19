using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols
{
    public interface ILinemodeTelnet
        : IAnsiProtocol
    {
        Task SendDoAsync(TelnetOption option);

        Task SendDontAsync(TelnetOption option);

        Task SendWillAsync(TelnetOption option);

        Task SendWontAsync(TelnetOption option);
    }
}
