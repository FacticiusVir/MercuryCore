using System;

namespace Keeper.MercuryCore
{
    internal interface IHostStartup
    {
        void Run(IServiceProvider serviceProvider);
    }
}
