using Keeper.MercuryCore.Pipeline;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.WebSockets
{
    public class WebSocketConnection
        : IConnection
    {
        private readonly string endpointName;
        private WebSocket socket;
        private readonly long connectionTime;

        private readonly ActionBlock<ArraySegment<byte>> sendBlock;
        private readonly BufferBlock<(ArraySegment<byte>, bool)> receiveBlock;

        private bool isClosed;
        private readonly TaskCompletionSource<object> closed = new TaskCompletionSource<object>();

        public WebSocketConnection(string endpointName, WebSocket socket)
        {
            this.endpointName = endpointName;
            this.socket = socket;

            this.connectionTime = DateTime.UtcNow.Ticks;

            this.sendBlock = new ActionBlock<ArraySegment<byte>>(this.SendAsync, new ExecutionDataflowBlockOptions { BoundedCapacity = 1, MaxDegreeOfParallelism = 1 });

            this.receiveBlock = new BufferBlock<(ArraySegment<byte>, bool)>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });

            this.BeginReceive();
        }

        public ITargetBlock<ArraySegment<byte>> Send => this.sendBlock;

        public IReceivableSourceBlock<(ArraySegment<byte>, bool)> Receive => this.receiveBlock;

        public Task Closed => this.closed.Task;

        public string UniqueIdentifier => $"{this.endpointName}/{this.connectionTime}";

        public string EndpointName => this.endpointName;

        public ConnectionType Type => ConnectionType.Chunked;

        private async Task SendAsync(ArraySegment<byte> data)
        {
            try
            {
                await this.socket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch
            {
                this.Close();
            }
        }

        private void BeginReceive()
        {
            Task.Run(async () =>
            {
                try
                {
                    var data = new byte[1024];

                    var result = await this.socket.ReceiveAsync(new ArraySegment<byte>(data), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        this.Close();
                    }
                    else
                    {
                        await this.receiveBlock.SendAsync((new ArraySegment<byte>(data, 0, result.Count), result.EndOfMessage));

                        this.BeginReceive();
                    }
                }
                catch
                {
                    this.Close();
                }
            });
        }

        public void Close()
        {
            if (!this.isClosed)
            {
                this.isClosed = true;

                if (this.socket.State != WebSocketState.Closed)
                {
                    this.socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None).Wait();
                }

                this.socket.Dispose();
                this.socket = null;

                this.closed.SetResult(null);
            }
        }
    }
}
