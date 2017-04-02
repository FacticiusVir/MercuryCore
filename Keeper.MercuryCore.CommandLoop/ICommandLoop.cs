namespace Keeper.MercuryCore.CommandLoop
{
    public interface ICommandLoop
    {
        bool IsRunning
        {
            get;
            set;
        }
    }
}
