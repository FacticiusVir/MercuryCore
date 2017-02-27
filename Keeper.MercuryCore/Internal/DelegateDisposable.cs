using System;

namespace Keeper.MercuryCore.Internal
{
    public class DelegateDisposable
        : IDisposable
    {
        private readonly Action disposeAction;

        public DelegateDisposable(Action disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        public void Dispose()
        {
            this.disposeAction();
        }
    }
}
