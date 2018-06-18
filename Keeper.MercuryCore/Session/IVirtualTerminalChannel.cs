using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session
{
    public interface IVirtualTerminalChannel
    {
        Task SendEscapeSequenceAsync(string sequence);
    }
}
