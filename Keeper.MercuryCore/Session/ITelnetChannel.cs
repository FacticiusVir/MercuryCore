using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Session
{
    public interface ITelnetChannel
    {
        Task SendCommandAsync(TelnetCommand command, TelnetOption option);

        IReceivableSourceBlock<(TelnetCommand Command, TelnetOption Option)> Negotiation { get; }

        IReceivableSourceBlock<(TelnetOption Option, IReceivableSourceBlock<byte> Data)> SubNegotiation { get; }
    }
}
