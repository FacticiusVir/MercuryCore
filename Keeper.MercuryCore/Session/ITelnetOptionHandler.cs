using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session
{
    public interface ITelnetOptionHandler
    {
        Task Handle(TelnetCommand command, TelnetOption option, ITelnetChannel channel);
    }
}
