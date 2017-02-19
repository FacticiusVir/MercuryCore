using Keeper.DotMudCore.Dataflow;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.DotMudCore.Protocols.Internal
{
    internal class PlainAscii
        : IPlainAscii
    {
        private readonly IConnection connection;
        private readonly IProtocolManagerControl protocolControl;

        private readonly IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;

        public PlainAscii(ILogger<PlainAscii> logger, IProtocolManagerControl protocolControl, IConnection connection)
        {
            this.connection = connection;
            this.protocolControl = protocolControl;

            this.lineAccumulator = LineAccumulatorBlock.Create(logger);
        }

        public async Task<string> ReceiveLineAsync()
        {
            await MakeActiveAsync();

            var tokenSource = new CancellationTokenSource();

            var receiveTask = this.lineAccumulator.ReceiveAsync(tokenSource.Token);

            Task.WaitAny(this.connection.Closed, receiveTask);

            if (this.connection.Closed.IsCompleted)
            {
                tokenSource.Cancel();

                throw new ClientDisconnectedException();
            }
            else
            {
                return receiveTask.Result;
            }
        }

        public async Task SendAsync(string message)
        {
            await MakeActiveAsync();

            await this.connection.Send.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message)));
        }

        public async Task MakeActiveAsync()
        {
            await this.protocolControl.MakeActiveAsync(this);
        }
        
        IDisposable IProtocol.CreateActiveSession()
        {
            return this.connection.Receive.LinkTo(this.lineAccumulator, new DataflowLinkOptions { PropagateCompletion = true });
        }
    }
}
