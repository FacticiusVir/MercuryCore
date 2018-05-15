using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Internal
{
    internal static class LineAccumulatorBlock
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
