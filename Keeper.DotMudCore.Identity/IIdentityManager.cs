using System.Threading.Tasks;

namespace Keeper.DotMudCore.Identity
{
    public interface IIdentityManager
    {
        Task<AuthenticateResult> Authenticate(ISession session);
    }
}
