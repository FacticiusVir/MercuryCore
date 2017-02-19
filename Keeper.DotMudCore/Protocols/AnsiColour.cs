namespace Keeper.DotMudCore.Protocols
{
    public enum AnsiColour
    {
        Black = 0,
        Red = 1,
        Green = 2,
        Yellow = Red | Green,
        Blue = 4,
        Magenta = Red | Blue,
        Cyan = Green | Blue,
        White = Red | Green | Blue
    }
}