using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Pipeline
{
    public interface IMiddleware
    {
        Func<Task> BuildHandler(IServiceProvider serviceProvider, Func<Task> next);
    }
}
