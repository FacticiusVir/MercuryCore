using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Pipeline
{
    public class DelegateMiddleware
        : IMiddleware
    {
        private readonly Func<IServiceProvider, Func<Task>, Func<Task>> buildFunction;

        public DelegateMiddleware(Func<IServiceProvider, Func<Task>, Func<Task>> buildFunction)
        {
            this.buildFunction = buildFunction;
        }

        public Func<Task> BuildHandler(IServiceProvider serviceProvider, Func<Task> next)
        {
            return this.buildFunction(serviceProvider, next);
        }
    }
}
