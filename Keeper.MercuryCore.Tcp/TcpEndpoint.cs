using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Tcp
{
    public class TcpEndpoint
        : IEndpoint
    {
        private readonly ILogger<TcpEndpoint> logger;
        private readonly TcpListener listener;
        private readonly List<TcpConnection> connections = new List<TcpConnection>();
        private readonly object connectionsLock = new object();

        public TcpEndpoint(IOptions<TcpOptions> options, ILogger<TcpEndpoint> logger)
        {
            this.logger = logger;
            this.listener = new TcpListener(IPAddress.Any, options.Value.Port);

            this.logger.LogInformation("TCP Endpoint configured on port {Port}", options.Value.Port);
        }

        public event Func<IConnection, Task> NewConnection;

        public void Start()
        {
            this.listener.Start();

            this.BeginAccept();
        }

        public void Stop()
        {
            this.listener.Stop();

            lock (this.connectionsLock)
            {
                foreach (var session in this.connections)
                {
                    session.Close();
                }
            }

            this.connections.Clear();
        }

        private void BeginAccept()
        {
            Task.Run(async () =>
            {
                TcpConnection newConnection = null;

                try
                {
                    var client = await this.listener.AcceptTcpClientAsync();

                    this.BeginAccept();

                    newConnection = new TcpConnection(client);

                    this.connections.Add(newConnection);

                    await this.NewConnection?.Invoke(newConnection);
                }
                catch(Exception ex)
                {
                    this.logger.LogError("Exception thrown from TCP Endpoint listener {Exception}", ex);
                }

                if (newConnection != null)
                {
                    lock (this.connectionsLock)
                    {
                        this.connections.Remove(newConnection);
                    }
                }
            });
        }
    }
}
