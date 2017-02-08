using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface ILoginManager
    {
        Task<LoginResult> Login(IConnection connection);
    }
}
