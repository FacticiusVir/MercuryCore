using Keeper.MercuryCore.Session;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Identity
{
    public interface IIdentityManager
    {
        Task<AuthenticateResult> Authenticate(ITextChannel channel);
    }
}
