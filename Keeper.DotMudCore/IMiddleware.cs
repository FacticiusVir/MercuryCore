using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public interface IMiddleware
    {
        Task Invoke(ISession session);
    }
}