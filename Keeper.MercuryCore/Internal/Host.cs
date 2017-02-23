using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Internal
{
    internal class Host
        : IHost
    {
        public Task RunAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
