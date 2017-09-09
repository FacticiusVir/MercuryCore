using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Identity
{
    public interface IIdentityPersistance
    {
        Task PersistAsync(IServiceProvider sessionProvider);
    }
}
