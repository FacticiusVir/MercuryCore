using Keeper.MercuryCore.Session;
using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Identity
{
    public interface IIdentityManager
    {
        Task<AuthenticateResult> AuthenticateAsync(IServiceProvider sessionProvider);
    }
}
