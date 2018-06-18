using System;
using System.Threading.Tasks;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Internal;
using System.Threading.Tasks.Dataflow;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Collections.Generic;

namespace Keeper.MercuryCore.Session.Internal
{
    internal class PlainTextChannel
        : ITextChannel, IChannel
    {
        private readonly ILogger<PlainTextChannel> logger;
        private readonly Encoding textEncoding;
        private readonly IPropagatorBlock<ArraySegment<byte>, string> lineAccumulator;
        private Func<ArraySegment<byte>, Task> send;

        public PlainTextChannel(ILogger<PlainTextChannel> logger, Encoding textEncoding)
        {
            this.logger = logger;
            this.textEncoding = textEncoding;

            var encodingBuffer = new BufferBlock<string>();
            var characterBuffer = new Queue<char>();

            var encodingAction = new ActionBlock<ArraySegment<byte>>(async data =>
            {
                var chars = this.textEncoding.GetChars(data.Array, data.Offset, data.Count);

                foreach (char character in chars)
                {
                    if (character == '\n')
                    {
                        var line = new string(characterBuffer.ToArray());

                        this.logger.LogTrace("Received line {Line}", line);

                        await encodingBuffer.SendAsync(line);

                        characterBuffer.Clear();
                    }
                    else if (character == '\r')
                    {
                        //Ignore
                    }
                    else
                    {
                        characterBuffer.Enqueue(character);
                    }
                }
            });

            this.lineAccumulator = DataflowBlock.Encapsulate(encodingAction, encodingBuffer);
        }

        public Func<ArraySegment<byte>, Task> Bind(Func<ArraySegment<byte>, Task> send)
        {
            this.send = send;

            return send;
        }

        public void Handle(byte datum, Action<byte> nextHandle, Action<SignalType> nextSignal)
        {
            this.lineAccumulator.Post(new ArraySegment<byte>(new[] { datum }));
        }

        public async Task<string> ReceiveLineAsync()
        {
            return await this.lineAccumulator.ReceiveAsync();
        }

        public Task SendLineAsync(string message)
        {
            var data = this.textEncoding.GetBytes(message + "\r\n");

            return send(new ArraySegment<byte>(data));
        }

        public void Signal(SignalType type, Action<SignalType> next)
        {
            if (type == SignalType.ConnectionClosed)
            {
                this.lineAccumulator.Complete();
            }

            next(type);
        }
    }
}
