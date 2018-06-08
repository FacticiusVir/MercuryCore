using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Session
{
    public interface ITelnetChannel
        : ITextChannel
    {
        Task SendCommandAsync(TelnetCommand command, TelnetOption option);

        IReceivableSourceBlock<(TelnetCommand Command, TelnetOption Option)> Negotiation { get; }

        IReceivableSourceBlock<(TelnetOption Option, IReceivableSourceBlock<byte> Data)> SubNegotiation { get; }

        IReceivableSourceBlock<char> ReceiveCharacter { get; }
    }
}
