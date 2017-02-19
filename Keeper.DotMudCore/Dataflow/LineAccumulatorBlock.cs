using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Keeper.DotMudCore.Dataflow
{
    public static class LineAccumulatorBlock
    {
        public static IPropagatorBlock<ArraySegment<byte>, string> Create(ILogger logger, IDictionary<byte, Func<Func<byte, bool>>> escapeHandlers = null)
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
                    else if (datum == '\b')
                    {
                        receiveBuffer.Pop();
                    }
                    else if (datum == '\n')
                    {
                        if (receiveBuffer.Peek() == '\r')
                        {
                            receiveBuffer.Pop();
                        }

                        var line = Encoding.ASCII.GetString(receiveBuffer.Reverse().ToArray());

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
