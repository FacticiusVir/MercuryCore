using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Pipeline
{
    public interface IEndpoint
    {
        event Func<IConnection, Task> NewConnection;

        void Start();

        void Stop();
    }
}
