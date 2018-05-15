namespace Keeper.MercuryCore.Session
{
    public enum TelnetCommand
        : byte
    {
        SE = 240,
        NOP = 241,
        DM = 242,
        BRK = 243,
        IP = 244,
        AO = 245,
        AYT = 246,
        EC = 247,
        EL = 248,
        GA = 249,
        SB = 250,
        WILL = 251,
        WONT = 252,
        DO = 253,
        DONT = 254,
        IAC = 255
    }

    public static class TelnetCommandExtensions
    {
        public static bool IsNegotiation(this TelnetCommand command)
        {
            switch (command)
            {
                case TelnetCommand.WILL:
                case TelnetCommand.WONT:
                case TelnetCommand.DO:
                case TelnetCommand.DONT:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPositive(this TelnetCommand command)
        {
            switch (command)
            {
                case TelnetCommand.WILL:
                case TelnetCommand.DO:
                    return true;
                default:
                    return false;
            }
        }

        public static TelnetCommand AsPositive(this TelnetCommand command)
        {
            if (!command.IsNegotiation() || command.IsPositive())
            {
                return command;
            }
            else
            {
                return command.Negate();
            }
        }

        public static TelnetCommand AsNegative(this TelnetCommand command)
        {
            if (!command.IsPositive())
            {
                return command;
            }
            else
            {
                return command.Negate();
            }
        }

        public static TelnetCommand Reciprocal(this TelnetCommand command)
        {
            switch (command)
            {
                case TelnetCommand.WILL:
                    return TelnetCommand.DO;
                case TelnetCommand.WONT:
                    return TelnetCommand.DONT;
                case TelnetCommand.DO:
                    return TelnetCommand.WILL;
                case TelnetCommand.DONT:
                    return TelnetCommand.WONT;
                default:
                    return command;
            }
        }

        public static TelnetCommand Negate(this TelnetCommand command)
        {
            switch (command)
            {
                case TelnetCommand.WILL:
                    return TelnetCommand.WONT;
                case TelnetCommand.WONT:
                    return TelnetCommand.WILL;
                case TelnetCommand.DO:
                    return TelnetCommand.DONT;
                case TelnetCommand.DONT:
                    return TelnetCommand.DO;
                default:
                    return command;
            }
        }
    }
}
