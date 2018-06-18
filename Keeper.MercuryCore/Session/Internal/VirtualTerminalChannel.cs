using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session.Internal
{
    internal class VirtualTerminalChannel
        : IVirtualTerminalChannel, IChannel
    {
        private Func<ArraySegment<byte>, Task> send;

        public Func<ArraySegment<byte>, Task> Bind(Func<ArraySegment<byte>, Task> send)
        {
            this.send = send;

            return send;
        }

        public void Handle(byte datum, Action<byte> next)
        {
            next(datum);
        }

        public async Task SendEscapeSequenceAsync(string sequence)
        {
            await send(Encoding.ASCII.GetBytes((char)0x1b + sequence));
        }
    }
}
