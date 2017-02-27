using Keeper.MercuryCore.Pipeline;
using System;

namespace Keeper.MercuryCore.Channel
{
    public interface IChannel
    {
        IDisposable Bind(IConnection connection);
    }
}
