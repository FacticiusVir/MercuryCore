using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Pipeline
{
    public interface IConnection
    {
        ITargetBlock<ArraySegment<byte>> Send { get; }

        IReceivableSourceBlock<(ArraySegment<byte>, bool)> Receive { get; }

        Task Closed { get; }

        void Close();

        string EndpointName { get; }

        string UniqueIdentifier { get; }

        ConnectionType Type { get; }
    }
}
