using System.Threading.Tasks;

namespace Keeper.DotMudCore.Identity
{
    public interface IUserManager
    {
        Task<bool> CheckUserAsync(string username, string password = null);

        Task<bool> CreateUserAsync(string username, string password);
    }
}
