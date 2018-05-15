using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session.Internal
{
    public class DefaultTelnetOptionHandler
        : ITelnetOptionHandler
    {
        public async Task Handle(TelnetCommand command, TelnetOption option, ITelnetChannel channel)
        {
            TelnetCommand response = command.Reciprocal().AsNegative();

            //if (option == TelnetOption.SuppressGoAhead && command.IsPositive())
            //{
            //    response = response.AsPositive();
            //}

            await channel.SendCommandAsync(response, option);
        }
    }
}
