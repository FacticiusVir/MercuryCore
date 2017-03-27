using System;
using System.Threading.Tasks;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Internal;
using System.Threading.Tasks.Dataflow;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Keeper.MercuryCore.Session.Internal
{
    internal class AsciiChannel
        : ITextChannel
    {
        private readonly ILogger<AsciiChannel> logger;
        private IConnection connection;
        private IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;

        public AsciiChannel(ILogger<AsciiChannel> logger)
        {
            this.logger = logger;
        }

        public IDisposable Bind(IConnection connection)
        {
            this.connection = connection;

            this.lineAccumulator = LineAccumulatorBlock.Create(this.logger, Encoding.ASCII);

            return this.connection.Receive.LinkTo(this.lineAccumulator);
        }

        public async Task<string> ReceiveLineAsync()
        {
            var tokenSource = new CancellationTokenSource();

            var receiveTask = this.lineAccumulator.ReceiveAsync(tokenSource.Token);

            await Task.WhenAny(this.connection.Closed, receiveTask);

            if (this.connection.Closed.IsCompleted)
            {
                tokenSource.Cancel();

                throw new ClientDisconnectedException();
            }
            else
            {
                return await receiveTask;
            }
        }

        public Task SendAsync(string message)
        {
            var data = Encoding.ASCII.GetBytes(message);

            return this.connection.Send.SendAsync(new ArraySegment<byte>(data));
        }
    }
}
