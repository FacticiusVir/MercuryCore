using System;

namespace Keeper.MercuryCore.CommandLoop
{
    public interface ICommandLoop
    {
        bool IsRunning
        {
            get;
            set;
        }

        IServiceProvider Provider
        {
            get;
        }
    }
}
