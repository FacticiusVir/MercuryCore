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
        : ITelnetChannel, IChannel
    {
        private readonly ILogger<TelnetChannel> logger;
        
        private readonly BufferBlock<(TelnetCommand, TelnetOption)> negotiationOutput = new BufferBlock<(TelnetCommand, TelnetOption)>();
        private readonly BufferBlock<(TelnetOption, IReceivableSourceBlock<byte>)> subnegotiationOutput = new BufferBlock<(TelnetOption, IReceivableSourceBlock<byte>)>();

        private ReceiveState receiveState = ReceiveState.Character;
        private TelnetCommand receivedCommand;
        private BufferBlock<byte> sbDataBuffer;
        private Func<ArraySegment<byte>, Task> send;

        private enum ReceiveState
        {
            Character,
            Escaped,
            Negotiation,
            SbInitial,
            SbData,
            SbEscaped
        }

        public TelnetChannel(ILogger<TelnetChannel> logger)
        {
            this.logger = logger;
        }

        public void Handle(byte datum, Action<byte> next)
        {
            switch (this.receiveState)
            {
                case ReceiveState.Character:
                    if ((TelnetCommand)datum == TelnetCommand.IAC)
                    {
                        this.receiveState = ReceiveState.Escaped;
                    }
                    else
                    {
                        next(datum);
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
                            next(0xff);
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
                    this.negotiationOutput.SendAsync((this.receivedCommand, (TelnetOption)datum));
                    this.receiveState = ReceiveState.Character;
                    break;
                case ReceiveState.SbInitial:
                    this.receiveState = ReceiveState.SbData;
                    this.sbDataBuffer = new BufferBlock<byte>();
                    this.subnegotiationOutput.SendAsync(((TelnetOption)datum, this.sbDataBuffer));
                    break;
                case ReceiveState.SbData:
                    if ((TelnetCommand)datum == TelnetCommand.IAC)
                    {
                        this.receiveState = ReceiveState.SbEscaped;
                    }
                    else
                    {
                        this.sbDataBuffer.SendAsync(datum);
                    }
                    break;
                case ReceiveState.SbEscaped:
                    if ((TelnetCommand)datum == TelnetCommand.IAC)
                    {
                        this.sbDataBuffer.SendAsync((byte)0xff);
                        this.receiveState = ReceiveState.SbData;
                    }
                    else
                    {
                        this.sbDataBuffer.Complete();
                        this.sbDataBuffer = null;

                        this.receiveState = ReceiveState.Character;
                    }
                    break;
            }
        }

        public IReceivableSourceBlock<(TelnetCommand, TelnetOption)> Negotiation => this.negotiationOutput;

        public IReceivableSourceBlock<(TelnetOption, IReceivableSourceBlock<byte>)> SubNegotiation => this.subnegotiationOutput;
        
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
            await send(data);
        }

        public Func<ArraySegment<byte>, Task> Bind(Func<ArraySegment<byte>, Task> send)
        {
            this.send = send;

            return send;
        }
    }
}
