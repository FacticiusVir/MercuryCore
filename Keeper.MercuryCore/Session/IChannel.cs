using Keeper.MercuryCore.Pipeline;
using System;

namespace Keeper.MercuryCore.Session
{
    public interface IChannel
    {
        IDisposable Bind(IConnection connection);
    }
}
