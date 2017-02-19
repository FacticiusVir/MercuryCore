using System;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols
{
    public interface IAnsiProtocol
        : IProtocol
    {
    }

    public static class AnsiProtocolExtensions
    {
        public static Task SendSetForegroundColourAsync(this IAnsiProtocol protocol, AnsiColour colour)
        {
            return SendSetColourAsync(protocol, 3, colour);
        }

        public static Task SendSetBackgroundColourAsync(this IAnsiProtocol protocol, AnsiColour colour)
        {
            return SendSetColourAsync(protocol, 4, colour);
        }

        private static Task SendSetColourAsync(IAnsiProtocol protocol, int modePrefix, AnsiColour colour)
        {
            if (!Enum.IsDefined(typeof(AnsiColour), colour))
            {
                throw new ArgumentOutOfRangeException("colour");
            }

            return protocol.SendAsync($"\x1b[{modePrefix}{(int)colour}m");
        }

        public async static Task SendAsync(this IAnsiProtocol protocol, AnsiColour fontColour, string message, AnsiColour revertFontColour = AnsiColour.White)
        {
            await protocol.SendSetForegroundColourAsync(fontColour);
            await protocol.SendAsync(message);
            await protocol.SendSetForegroundColourAsync(revertFontColour);
        }
    }
}
