namespace Keeper.DotMudCore.CommandLoop.Parsing
{
    public interface ICommandParser
    {
        CommandInfo Parse(string line);
    }
}
