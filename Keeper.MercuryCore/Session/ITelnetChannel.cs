using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Session
{
    public interface ITelnetChannel
        : ITextChannel
    {
        Task SendCommandAsync(TelnetCommand command, TelnetOption option);

        IReceivableSourceBlock<(TelnetCommand, TelnetOption)> Negotiation { get; }

        IReceivableSourceBlock<(TelnetOption, IReceivableSourceBlock<byte>)> SubNegotiation { get; }

        IReceivableSourceBlock<char> ReceiveCharacter { get; }
    }
}
