using System;

namespace Keeper.DotMudCore
{
    public interface IEndpoint
    {
        void Start();

        void Stop();

        event Action<IConnection> NewConnection;
    }
}
