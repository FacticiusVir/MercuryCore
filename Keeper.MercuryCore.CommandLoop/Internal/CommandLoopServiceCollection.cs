using Microsoft.Extensions.DependencyInjection;

namespace Keeper.MercuryCore.CommandLoop.Internal
{
    internal class CommandLoopServiceCollection
        : ICommandLoopServiceCollection
    {
        private IServiceCollection wrappedCollection;

        public CommandLoopServiceCollection(IServiceCollection wrappedCollection)
        {
            this.wrappedCollection = wrappedCollection;
        }

        public ICommandLoopServiceCollection AddSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this.wrappedCollection.AddSingleton<TService, TImplementation>();

            return this;
        }
    }
}
