using System.Linq;

namespace Keeper.MercuryCore.CommandLoop.Parsing.Internal
{
    internal class VerbObjectParser
        : ICommandParser
    {
        public CommandInfo Parse(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return CommandInfo.Invalid;
            }
            else
            {
                var parts = line.Split(' ');

                return new CommandInfo(parts[0].ToUpperInvariant(), parts.Skip(1).ToArray());
            }
        }
    }
}
