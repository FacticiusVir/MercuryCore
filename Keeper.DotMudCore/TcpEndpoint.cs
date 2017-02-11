using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class TcpEndpoint
        : IEndpoint
    {
        private readonly ILogger<TcpEndpoint> logger;
        private readonly TcpListener listener;
        private readonly List<IConnection> connections = new List<IConnection>();

        public TcpEndpoint(IOptions<TcpOptions> options, ILogger<TcpEndpoint> logger)
        {
            this.logger = logger;
            this.listener = new TcpListener(IPAddress.Any, options.Value.Port);

            this.logger.LogInformation("TCP Endpoint configured on port {Port}", options.Value.Port);
        }

        public event Action<IConnection> NewConnection;

        public void Start()
        {
            this.listener.Start();

            this.BeginAccept();
        }

        public void Stop()
        {
            this.listener.Stop();

            foreach (var session in this.connections)
            {
                session.Close();
            }

            this.connections.Clear();
        }

        private void BeginAccept()
        {
            Task.Run(async () =>
            {
                var client = await this.listener.AcceptTcpClientAsync();

                this.BeginAccept();

                var newConnection = new TcpConnection(client);

                this.connections.Add(newConnection);

                this.NewConnection?.Invoke(newConnection);
            });
        }
    }
}
