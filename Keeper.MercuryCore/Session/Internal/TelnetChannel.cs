using Keeper.MercuryCore.Internal;
using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Session.Internal
{
    internal class TelnetChannel
        : ITextChannel
    {
        private readonly ILogger<TelnetChannel> logger;
        private IConnection connection;
        private IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;

        public TelnetChannel(ILogger<TelnetChannel> logger)
        {
            this.logger = logger;
        }

        public IDisposable Bind(IConnection connection)
        {
            this.connection = connection;

            this.lineAccumulator = LineAccumulatorBlock.Create(logger, new Dictionary<byte, Func<Func<byte, bool>>>
            {
                { (byte)0xff, this.IacHandler}
            });

            return this.connection.Receive.LinkTo(this.lineAccumulator);
        }



        private Func<byte, bool> IacHandler()
        {
            List<byte> bytes = null;

            return datum =>
            {
                if (bytes == null)
                {
                    bytes = new List<byte>();
                }

                bytes.Add(datum);

                if (bytes.Count == 2)
                {
                    TelnetOption option = (TelnetOption)bytes[1];

                    this.logger.LogDebug($"Received IAC {(TelnetCommand)bytes[0]} {option}");

                    return true;
                }

                return false;
            };
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
