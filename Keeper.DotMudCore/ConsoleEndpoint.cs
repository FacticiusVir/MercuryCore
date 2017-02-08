using System;
using System.Threading;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class ConsoleEndpoint
        : IEndpoint, IConnection
    {
        public WaitHandle ClosedHandle => this.closedHandle;

        public string UniqueIdentifier => "Console Endpoint";

        public event Action<IConnection> NewConnection;

        private ManualResetEvent closedHandle = new ManualResetEvent(false);

        public void Start()
        {
            Task.Run(() => this.NewConnection?.Invoke(this));
        }

        public void Stop()
        {
        }

        void IConnection.Close()
        {
            this.closedHandle.Set();
        }

        Task<string> IConnection.ReceiveAsync()
        {
            return Task.Run(() => Console.ReadLine());
        }

        Task IConnection.SendAsync(string message)
        {
            return Task.Run(() => Console.WriteLine($" >> {message}"));
        }
    }
}
