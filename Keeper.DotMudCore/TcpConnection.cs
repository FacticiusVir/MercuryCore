using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class TcpConnection
        : IConnection
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private readonly long connectionTime;

        private bool isClosed;

        public TcpConnection(TcpClient client)
        {
            this.client = client;

            this.RemoteEndPoint = this.client.Client.RemoteEndPoint;

            var stream = client.GetStream();

            this.reader = new StreamReader(stream);
            this.writer = new StreamWriter(stream);

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

        public async Task SendLineAsync(string message)
        {
            try
            {
                await this.writer.WriteLineAsync(message);
                await this.writer.FlushAsync();
            }
            catch (Exception ex)
            {
                this.Close();

                throw new ClientDisconnectedException(ex);
            }
        }

        public async Task<string> ReceiveLineAsync()
        {
            string message = null;

            try
            {
                message = await this.reader.ReadLineAsync();
            }
            catch (Exception ex)
            {
                this.Close();

                throw new ClientDisconnectedException(ex);
            }

            if (message == null)
            {
                this.Close();

                throw new ClientDisconnectedException();
            }
            else
            {
                return Sanitise(message);
            }
        }

        private string Sanitise(string message)
        {
            var builder = new StringBuilder();

            foreach (char character in message)
            {
                if (character == '\b')
                {
                    if (builder.Length > 0)
                    {
                        builder.Length--;
                    }
                }
                else
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
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
