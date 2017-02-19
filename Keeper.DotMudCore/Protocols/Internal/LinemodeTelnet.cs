using Keeper.DotMudCore.Dataflow;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.DotMudCore.Protocols.Internal
{
    internal class LinemodeTelnet
        : ILinemodeTelnet
    {
        private readonly ILogger<LinemodeTelnet> logger;
        private readonly IConnection connection;
        private readonly IProtocolManagerControl protocolControl;

        private readonly IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;

        public LinemodeTelnet(ILogger<LinemodeTelnet> logger, IProtocolManagerControl protocolControl, IConnection connection)
        {
            this.logger = logger;
            this.connection = connection;
            this.protocolControl = protocolControl;

            this.lineAccumulator = LineAccumulatorBlock.Create(logger, new Dictionary<byte, Func<Func<byte, bool>>>
            {
                { (byte)0xff, this.IacHandler}
            });
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

        public Task SendDoAsync(TelnetOption option)
        {
            return this.SendOptionAsync(TelnetCommand.DO, option);
        }

        public Task SendDontAsync(TelnetOption option)
        {
            return this.SendOptionAsync(TelnetCommand.DONT, option);
        }

        public Task SendWillAsync(TelnetOption option)
        {
            return this.SendOptionAsync(TelnetCommand.WILL, option);
        }

        public Task SendWontAsync(TelnetOption option)
        {
            return this.SendOptionAsync(TelnetCommand.WONT, option);
        }

        private async Task SendOptionAsync(TelnetCommand command, TelnetOption option)
        {
            this.logger.LogDebug($"Sending IAC {command} {option}");

            await this.connection.Send.SendAsync(new ArraySegment<byte>(new byte[] { (byte)TelnetCommand.IAC, (byte)command, (byte)option }));
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
            return this.connection.Receive.LinkTo(this.lineAccumulator);
        }
    }
}
