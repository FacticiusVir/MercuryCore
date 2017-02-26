using Keeper.MercuryCore.Pipeline;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Tcp
{
    public class TcpConnection
        : IConnection
    {
        private TcpClient client;
        private NetworkStream stream;
        private readonly long connectionTime;

        private ActionBlock<ArraySegment<byte>> sendBlock;
        private BufferBlock<ArraySegment<byte>> receiveBlock;

        private bool isClosed;
        private TaskCompletionSource<object> closed = new TaskCompletionSource<object>();

        public TcpConnection(TcpClient client)
        {
            this.client = client;

            this.RemoteEndPoint = this.client.Client.RemoteEndPoint;

            this.stream = client.GetStream();

            this.connectionTime = DateTime.UtcNow.Ticks;

            this.sendBlock = new ActionBlock<ArraySegment<byte>>(this.SendAsync, new ExecutionDataflowBlockOptions { BoundedCapacity = 1, MaxDegreeOfParallelism = 1 });

            this.receiveBlock = new BufferBlock<ArraySegment<byte>>(new DataflowBlockOptions { BoundedCapacity = DataflowBlockOptions.Unbounded });

            this.BeginReceive();
        }

        public EndPoint RemoteEndPoint
        {
            get;
            private set;
        }

        public bool IsOpen
        {
            get
            {
                return !this.isClosed;
            }
        }

        public Task Closed => this.closed.Task;

        public string UniqueIdentifier => $"TCP/{this.RemoteEndPoint}/{this.connectionTime}";

        public ITargetBlock<ArraySegment<byte>> Send => this.sendBlock;

        public IReceivableSourceBlock<ArraySegment<byte>> Receive => this.receiveBlock;

        private async Task SendAsync(ArraySegment<byte> data)
        {
            try
            {
                await this.stream.WriteAsync(data.Array, data.Offset, data.Count);
                await this.stream.FlushAsync();
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

                    int count = await this.stream.ReadAsync(data, 0, data.Length);

                    if (count == 0)
                    {
                        this.Close();
                    }
                    else
                    {
                        await this.receiveBlock.SendAsync(new ArraySegment<byte>(data, 0, count));

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

                this.client.Dispose();
                this.client = null;

                this.closed.SetResult(null);
            }
        }
    }
}
