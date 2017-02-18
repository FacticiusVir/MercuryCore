using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class TcpConnection
        : IConnection
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private StreamWriter writer;
        private readonly long connectionTime;

        private bool isClosed;

        public TcpConnection(TcpClient client)
        {
            this.client = client;

            this.RemoteEndPoint = this.client.Client.RemoteEndPoint;

            this.stream = client.GetStream();

            this.writer = new StreamWriter(this.stream);
            this.reader = new StreamReader(this.stream);

            this.connectionTime = DateTime.UtcNow.Ticks;
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

        public string UniqueIdentifier => $"TCP/{this.RemoteEndPoint}/{this.connectionTime}";

        public async Task SendAsync(byte[] data, int offset, int count)
        {
            try
            {
                await this.stream.WriteAsync(data, offset, count);
                await this.stream.FlushAsync();
            }
            catch (Exception ex)
            {
                this.Close();

                throw new ClientDisconnectedException(ex);
            }
        }

        public async Task<int> ReceiveAsync(byte[] data, int offset, int count)
        {
            try
            {
                return await this.stream.ReadAsync(data, offset, count);
            }
            catch (Exception ex)
            {
                this.Close();

                throw new ClientDisconnectedException(ex);
            }
        }

        public async Task<int> ReceiveAsync(byte[] data, int offset, int count, CancellationToken token)
        {
            try
            {
                return await this.stream.ReadAsync(data, offset, count, token);
            }
            catch (Exception ex)
            {
                this.Close();

                throw new ClientDisconnectedException(ex);
            }
        }

        public void Close()
        {
            if (!this.isClosed)
            {
                this.isClosed = true;

                this.client.Dispose();
                this.client = null;
                this.reader = null;
                this.writer = null;
            }
        }
    }
}
