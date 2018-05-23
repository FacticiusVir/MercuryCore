using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Internal
{
    public class LineAccumulatorBlock
        : IDisposable
    {
        private readonly ILogger logger;
        private readonly Encoding encoding;
        private readonly IDictionary<byte, Func<Func<byte, bool>>> escapeHandlers;
        private readonly IDisposable binding;
        private readonly Stack<byte> receiveBuffer = new Stack<byte>();
        private readonly BufferBlock<string> output = new BufferBlock<string>();

        private readonly object modeLockObject = new object();

        private Func<byte, bool> currentEscapeHandler;

        public LineAccumulatorBlock(ILogger logger, Encoding encoding, ISourceBlock<ArraySegment<byte>> connection, IDictionary<byte, Func<Func<byte, bool>>> escapeHandlers = null)
        {
            this.logger = logger;
            this.encoding = encoding;
            this.escapeHandlers = escapeHandlers;

            var accumulator = new ActionBlock<ArraySegment<byte>>(async data =>
            {
                var linesToSend = new List<string>();

                lock(this.modeLockObject)
                {
                    this.logger.LogTrace("Received {ByteCount} bytes.", data.Count);

                    for (int index = data.Offset; index + data.Offset < data.Count; index++)
                    {
                        byte datum = data.Array[index];

                        if (this.currentEscapeHandler != null)
                        {
                            if (this.currentEscapeHandler(datum))
                            {
                                this.currentEscapeHandler = null;
                            }
                        }
                        else if (datum == '\n')
                        {
                            if (receiveBuffer.Peek() == '\r')
                            {
                                receiveBuffer.Pop();
                            }

                            var lineData = new Stack<byte>();

                            while (receiveBuffer.Any())
                            {
                                byte lineDatum = receiveBuffer.Pop();

                                if (lineDatum == '\b')
                                {
                                    if (receiveBuffer.Any())
                                    {
                                        receiveBuffer.Pop();
                                    }
                                }
                                else
                                {
                                    lineData.Push(lineDatum);
                                }
                            }

                            var line = encoding.GetString(lineData.ToArray());

                            linesToSend.Add(line);

                            receiveBuffer.Clear();
                        }
                        else if (escapeHandlers != null && escapeHandlers.TryGetValue(datum, out var getHandler))
                        {
                            currentEscapeHandler = getHandler();
                        }
                        else
                        {
                            receiveBuffer.Push(datum);
                        }
                    }
                }

                foreach(var line in linesToSend)
                {
                    await this.output.SendAsync(line);
                }
            });

            this.binding = connection.LinkTo(accumulator);
        }

        public IReceivableSourceBlock<string> Output => this.output;

        public void Dispose()
        {
            this.binding.Dispose();
        }
    }

    internal static class LineAccumulatorBlockOld
    {
        public static IPropagatorBlock<ArraySegment<byte>, string> Create(ILogger logger, Encoding encoding, IDictionary<byte, Func<Func<byte, bool>>> escapeHandlers = null)
        {
            var receiveBuffer = new Stack<byte>();

            var output = new BufferBlock<string>();

            Func<byte, bool> currentEscapeHandler = null;

            var accumulator = new ActionBlock<ArraySegment<byte>>(async data =>
            {
                logger.LogTrace("Received {ByteCount} bytes.", data.Count);

                for (int index = data.Offset; index + data.Offset < data.Count; index++)
                {
                    byte datum = data.Array[index];

                    if (currentEscapeHandler != null)
                    {
                        if (currentEscapeHandler(datum))
                        {
                            currentEscapeHandler = null;
                        }
                    }
                    else if (datum == '\n')
                    {
                        if (receiveBuffer.Peek() == '\r')
                        {
                            receiveBuffer.Pop();
                        }

                        var lineData = new Stack<byte>();

                        while (receiveBuffer.Any())
                        {
                            byte lineDatum = receiveBuffer.Pop();

                            if (lineDatum == '\b')
                            {
                                if (receiveBuffer.Any())
                                {
                                    receiveBuffer.Pop();
                                }
                            }
                            else
                            {
                                lineData.Push(lineDatum);
                            }
                        }

                        var line = encoding.GetString(lineData.ToArray());

                        await output.SendAsync(line);

                        receiveBuffer.Clear();
                    }
                    else if (escapeHandlers != null && escapeHandlers.TryGetValue(datum, out var getHandler))
                    {
                        currentEscapeHandler = getHandler();
                    }
                    else
                    {
                        receiveBuffer.Push(datum);
                    }
                }
            }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            return DataflowBlock.Encapsulate(accumulator, output);
        }
    }
}
