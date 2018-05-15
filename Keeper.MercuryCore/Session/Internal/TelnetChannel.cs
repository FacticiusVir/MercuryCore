using Keeper.MercuryCore.Internal;
using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Session.Internal
{
    internal class TelnetChannel
        : ITelnetChannel
    {
        private readonly ILogger<TelnetChannel> logger;
        private readonly Encoding encoding;
        private readonly ITelnetOptionHandler optionHandler;
        private IConnection connection;
        private IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;

        public TelnetChannel(ILogger<TelnetChannel> logger, Encoding encoding, ITelnetOptionHandler optionHandler = null)
        {
            this.logger = logger;
            this.encoding = encoding;
            this.optionHandler = optionHandler ?? new DefaultTelnetOptionHandler();
        }

        public IDisposable Bind(IConnection connection)
        {
            this.connection = connection;

            this.lineAccumulator = LineAccumulatorBlock.Create(logger, encoding, new Dictionary<byte, Func<Func<byte, bool>>>
            {
                { (byte)0xff, this.HandleIac}
            });

            return this.connection.Receive.LinkTo(this.lineAccumulator);
        }

        private Func<byte, bool> HandleIac()
        {
            List<byte> bytes = null;

            return datum =>
            {
                if (bytes == null)
                {
                    bytes = new List<byte>();
                }

                bytes.Add(datum);

                TelnetCommand command = (TelnetCommand)bytes[0];

                if (command.IsNegotiation())
                {
                    if (bytes.Count == 2)
                    {
                        TelnetOption option = (TelnetOption)bytes[1];

                        this.logger.LogDebug("Received IAC {TelnetCommand} {TelnetOption}", command, option);

                        this.optionHandler.Handle(command, option, this);

                        return true;
                    }
                }
                else if (command == TelnetCommand.SB)
                {
                    if ((TelnetCommand)bytes.Last() == TelnetCommand.SE && (TelnetCommand)bytes.SkipLast(1).Last() == TelnetCommand.IAC)
                    {
                        TelnetOption option = (TelnetOption)bytes[1];
                        TelnetSubNegotiationCommand subCommand = (TelnetSubNegotiationCommand)bytes[2];

                        var data = bytes.Skip(3).SkipLast(2);

                        this.logger.LogDebug("Received IAC {TelnetCommand} {TelnetOption} {TelnetSubNegotiationCommand} {SubNegotationData} IAC SE", command, option, subCommand, data);

                        return true;
                    }
                }
                else
                {
                    this.logger.LogDebug("Received IAC {TelnetCommand}", command);

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

        public Task SendLineAsync(string message)
        {
            var data = this.encoding.GetBytes(message + "\r\n");

            return this.connection.Send.SendAsync(new ArraySegment<byte>(data));
        }

        public Task SendCommandAsync(TelnetCommand command, TelnetOption option)
        {
            var data = new byte[] { 0xff, (byte)command, (byte)option };

            this.logger.LogDebug("Sending IAC {TelnetCommand} {TelnetOption}", command, option);

            return this.connection.Send.SendAsync(new ArraySegment<byte>(data));
        }
    }
}
