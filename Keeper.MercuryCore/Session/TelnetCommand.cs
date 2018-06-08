namespace Keeper.MercuryCore.Session
{
    public enum TelnetCommand
        : byte
    {
        /// <summary>
        /// SubNegotiation Ends
        /// </summary>
        SE = 240,
        /// <summary>
        /// No-Op
        /// </summary>
        NOP = 241,
        /// <summary>
        /// Data Mark
        /// </summary>
        DM = 242,
        /// <summary>
        /// Break
        /// </summary>
        BRK = 243,
        /// <summary>
        /// Interupt Process
        /// </summary>
        IP = 244,
        /// <summary>
        /// Abort Output
        /// </summary>
        AO = 245,
        /// <summary>
        /// Are You There
        /// </summary>
        AYT = 246,
        /// <summary>
        /// Erase Character
        /// </summary>
        EC = 247,
        /// <summary>
        /// Erase Line
        /// </summary>
        EL = 248,
        /// <summary>
        /// Go Ahead
        /// </summary>
        GA = 249,
        /// <summary>
        /// SubNegotiation
        /// </summary>
        SB = 250,
        /// <summary>
        /// Negotiation Will
        /// </summary>
        WILL = 251,
        /// <summary>
        /// Negotiation Wont
        /// </summary>
        WONT = 252,
        /// <summary>
        /// Negotiation Do
        /// </summary>
        DO = 253,
        /// <summary>
        /// Negotiation Dont
        /// </summary>
        DONT = 254,
        /// <summary>
        /// Interpret As Command
        /// </summary>
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

        public static bool IsImperative(this TelnetCommand command)
        {
            switch (command)
            {
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
