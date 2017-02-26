using System;

namespace Keeper.MercuryCore.Pipeline
{
    public interface IEndpoint
    {
        event Action<IConnection> NewConnection;

        void Start();

        void Stop();
    }
}
