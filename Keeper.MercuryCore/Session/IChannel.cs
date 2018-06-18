using Keeper.MercuryCore.Pipeline;
using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Session
{
    public interface IChannel
    {
        void Handle(byte datum, Action<byte> next);

        Func<ArraySegment<byte>, Task> Bind(Func<ArraySegment<byte>, Task> send);
    }
}
