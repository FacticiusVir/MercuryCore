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
    internal class PlainTextChannel
        : ITextChannel
    {
        private readonly ILogger<PlainTextChannel> logger;
        private readonly Encoding textEncoding;
        private IConnection connection;
        private IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;

        public PlainTextChannel(ILogger<PlainTextChannel> logger, Encoding textEncoding)
        {
            this.logger = logger;
            this.textEncoding = textEncoding;
        }

        public IDisposable Bind(IConnection connection)
        {
            this.connection = connection;

            if (connection.Type == ConnectionType.Stream)
            {
                this.lineAccumulator = LineAccumulatorBlockOld.Create(this.logger, this.textEncoding);
            }
            else
            {
                var encodingBuffer = new BufferBlock<string>();

                var encodingAction = new ActionBlock<ArraySegment<byte>>(async data =>
                {
                    string value = this.textEncoding.GetString(data.Array, data.Offset, data.Count);

                    await encodingBuffer.SendAsync(value);
                });

                this.lineAccumulator = DataflowBlock.Encapsulate(encodingAction, encodingBuffer);
            }

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

        public Task SendLineAsync(string message)
        {
            var data = Encoding.ASCII.GetBytes(message + "\r\n");

            return this.connection.Send.SendAsync(new ArraySegment<byte>(data));
        }
    }
}
