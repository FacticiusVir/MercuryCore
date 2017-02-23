using System.Threading;
using System.Threading.Tasks;

namespace Keeper.MercuryCore
{
    public interface IHost
    {
        Task RunAsync(CancellationToken token);
    }
}
