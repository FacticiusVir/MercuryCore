using System.Threading.Tasks;

namespace Keeper.DotMudCore.Identity
{
    public interface ILoginManager
    {
        Task<LoginResult> Login(IConnection connection);
    }
}
