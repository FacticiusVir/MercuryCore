using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
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
        private readonly X509Certificate serverCertificate;

        public TcpEndpoint(IOptions<TcpOptions> options, ILogger<TcpEndpoint> logger)
        {
            this.logger = logger;
            this.listener = new TcpListener(options.Value.Address, options.Value.Port);

            this.logger.LogInformation("TCP Endpoint configured on port {Port}", options.Value.Port);

            if (options.Value.SslCertValue != null)
            {
                using (var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine))
                {
                    certStore.Open(OpenFlags.OpenExistingOnly);

                    this.serverCertificate = certStore.Certificates.Find(options.Value.SslCertFind, options.Value.SslCertValue, false)[0];

                    this.logger.LogInformation("TCP Endpoint secured for subject {CertificateSubject}", this.serverCertificate.Subject);
                }
            }
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
                var connectionsToClose = this.connections.ToArray();

                foreach (var connection in connectionsToClose)
                {
                    connection.Close();
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

                    newConnection = new TcpConnection(client, this.serverCertificate);

                    this.connections.Add(newConnection);

                    await this.NewConnection?.Invoke(newConnection);
                }
                catch (Exception ex)
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
