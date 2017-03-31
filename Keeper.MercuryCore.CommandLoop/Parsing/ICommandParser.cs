namespace Keeper.MercuryCore.CommandLoop.Parsing
{
    public interface ICommandParser
    {
        CommandInfo Parse(string line);
    }
}
