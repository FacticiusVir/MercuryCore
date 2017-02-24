using System;

namespace Keeper.MercuryCore.Internal
{
    internal class DelegateHostStartup
        : IHostStartup
    {
        private Action<IServiceProvider> startupAction;

        public DelegateHostStartup(Action<IServiceProvider> startupAction)
        {
            this.startupAction = startupAction;
        }

        public void Run(IServiceProvider serviceProvider)
        {
            this.startupAction(serviceProvider);
        }
    }
}