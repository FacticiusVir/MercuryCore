using Keeper.MercuryCore.Internal;
using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
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
        private IConnection connection;

        private readonly Stack<byte> lineModeBuffer = new Stack<byte>();
        private readonly BufferBlock<string> lineOutput = new BufferBlock<string>();
        private readonly BufferBlock<(TelnetCommand, TelnetOption)> negotiationOutput = new BufferBlock<(TelnetCommand, TelnetOption)>();
        private readonly BufferBlock<(TelnetOption, IReceivableSourceBlock<byte>)> subnegotiationOutput = new BufferBlock<(TelnetOption, IReceivableSourceBlock<byte>)>();
        private readonly BufferBlock<char> characterOutput = new BufferBlock<char>();
        private readonly AsyncLock accumulateLock = new AsyncLock();

        private ReceiveState receiveState = ReceiveState.Character;
        private TelnetCommand receivedCommand;
        private BufferBlock<byte> sbDataBuffer;

        private enum ReceiveState
        {
            Character,
            Escaped,
            Negotiation,
            SbInitial,
            SbData,
            SbEscaped
        }

        public TelnetChannel(ILogger<TelnetChannel> logger, Encoding encoding)
        {
            this.logger = logger;
            this.encoding = encoding;
        }

        public IDisposable Bind(IConnection connection)
        {
            this.connection = connection;

            return this.connection.Receive.LinkTo(new ActionBlock<ArraySegment<byte>>(HandleData));
        }

        private async Task HandleData(ArraySegment<byte> data)
        {
            this.logger.LogDebug("Received {ByteCount} bytes.", data.Count);
            this.logger.LogTrace("Received {Payload}", data);

            using (await this.accumulateLock.LockAsync())
            {
                for (int index = data.Offset; index + data.Offset < data.Count; index++)
                {
                    byte datum = data.Array[index];

                    switch (this.receiveState)
                    {
                        case ReceiveState.Character:
                            if (datum == '\n')
                            {
                                if (this.lineModeBuffer.Peek() == '\r')
                                {
                                    this.lineModeBuffer.Pop();
                                }

                                var lineData = new Stack<byte>();

                                while (this.lineModeBuffer.Any())
                                {
                                    byte lineDatum = this.lineModeBuffer.Pop();

                                    if (lineDatum == '\b')
                                    {
                                        if (this.lineModeBuffer.Any())
                                        {
                                            this.lineModeBuffer.Pop();
                                        }
                                    }
                                    else
                                    {
                                        lineData.Push(lineDatum);
                                    }
                                }

                                var line = encoding.GetString(lineData.ToArray());

                                this.logger.LogTrace("Received line {Line}", line);
                                await this.lineOutput.SendAsync(line);

                                this.lineModeBuffer.Clear();
                            }
                            else if ((TelnetCommand)datum == TelnetCommand.IAC)
                            {
                                this.receiveState = ReceiveState.Escaped;
                            }
                            else
                            {
                                this.lineModeBuffer.Push(datum);
                            }
                            break;
                        case ReceiveState.Escaped:
                            TelnetCommand command = (TelnetCommand)datum;

                            switch (command)
                            {
                                case TelnetCommand.DO:
                                case TelnetCommand.DONT:
                                case TelnetCommand.WILL:
                                case TelnetCommand.WONT:
                                    this.receiveState = ReceiveState.Negotiation;
                                    this.receivedCommand = command;
                                    break;
                                case TelnetCommand.IAC:
                                    this.receiveState = ReceiveState.Character;
                                    this.lineModeBuffer.Push(0xff);
                                    break;
                                case TelnetCommand.SB:
                                    this.receiveState = ReceiveState.SbInitial;
                                    break;
                                default:
                                    this.receiveState = ReceiveState.Character;
                                    break;

                            }
                            break;
                        case ReceiveState.Negotiation:
                            this.logger.LogDebug("Received IAC {TelnetCommand} {TelnetOption}", this.receivedCommand, (TelnetOption)datum);
                            await this.negotiationOutput.SendAsync((this.receivedCommand, (TelnetOption)datum));
                            this.receiveState = ReceiveState.Character;
                            break;
                        case ReceiveState.SbInitial:
                            this.receiveState = ReceiveState.SbData;
                            this.sbDataBuffer = new BufferBlock<byte>();
                            await this.subnegotiationOutput.SendAsync(((TelnetOption)datum, this.sbDataBuffer));
                            break;
                        case ReceiveState.SbData:
                            if ((TelnetCommand)datum == TelnetCommand.IAC)
                            {
                                this.receiveState = ReceiveState.SbEscaped;
                            }
                            else
                            {
                                await this.sbDataBuffer.SendAsync(datum);
                            }
                            break;
                        case ReceiveState.SbEscaped:
                            if ((TelnetCommand)datum == TelnetCommand.IAC)
                            {
                                await this.sbDataBuffer.SendAsync((byte)0xff);
                                this.receiveState = ReceiveState.SbData;
                            }
                            else
                            {
                                this.sbDataBuffer.Complete();
                                this.sbDataBuffer = null;

                                this.receiveState = ReceiveState.Character;
                            }
                            break;
                        default:
                            this.connection.Close();
                            this.logger.LogError("Current receive state {ReceiveState} is not implemented.", this.receiveState);
                            return;
                    }

                }
            }
        }

        public IReceivableSourceBlock<(TelnetCommand, TelnetOption)> Negotiation => this.negotiationOutput;

        public IReceivableSourceBlock<(TelnetOption, IReceivableSourceBlock<byte>)> SubNegotiation => this.subnegotiationOutput;

        public IReceivableSourceBlock<char> ReceiveCharacter => this.characterOutput;

        public async Task<string> ReceiveLineAsync()
        {
            var tokenSource = new CancellationTokenSource();

            var receiveTask = this.lineOutput.ReceiveAsync(tokenSource.Token);

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

        public async Task SendLineAsync(string message)
        {
            var data = this.encoding.GetBytes(message + "\r\n");

            await this.SendData(data);
        }

        public async Task SendCommandAsync(TelnetCommand command, TelnetOption option)
        {
            var data = new byte[] { 0xff, (byte)command, (byte)option };

            this.logger.LogDebug("Sending IAC {TelnetCommand} {TelnetOption}", command, option);

            await SendData(data);
        }

        public async Task SendSubCommandAsync(TelnetOption option, byte[] subData)
        {
            var data = new byte[] { 0xff, (byte)TelnetCommand.SB, (byte)option }.Concat(subData).Concat(new byte[] { 0xff, (byte)TelnetCommand.SE }).ToArray();

            this.logger.LogDebug("Sending IAC {TelnetCommand} {TelnetOption}", TelnetCommand.SB, option);

            await SendData(data);
        }

        private async Task SendData(byte[] data)
        {
            this.logger.LogDebug("Sending {ByteCount} bytes.", data.Length);

            var payload = new ArraySegment<byte>(data);

            this.logger.LogTrace("Sending {Payload}", payload);

            await this.connection.Send.SendAsync(payload);
        }
    }
}
