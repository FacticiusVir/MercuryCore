using System;
using System.Threading.Tasks.Dataflow;

namespace Keeper.DotMudCore
{
    public interface IConnection
    {
        ITargetBlock<ArraySegment<byte>> Send { get; }
        
        IReceivableSourceBlock<ArraySegment<byte>> Receive { get; }

        void Close();

        string UniqueIdentifier { get; }
    }
}
