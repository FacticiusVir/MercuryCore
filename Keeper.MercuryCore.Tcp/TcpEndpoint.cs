using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using Keeper.MercuryCore.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System;

namespace Keeper.MercuryCore.Tcp
{
    public class TcpEndpoint
        : IEndpoint
    {
        private readonly ILogger<TcpEndpoint> logger;
        private readonly int port;

        public TcpEndpoint(ILogger<TcpEndpoint> logger, IOptions<TcpOptions> options)
        {
            this.logger = logger;
            this.port = options.Value.Port;

            this.logger.LogInformation("TCP Endpoint configured on port {Port}", this.port);
        }

        public void Start()
        {
            this.logger.LogInformation("TCP Endpoint started.");
        }

        public void Stop()
        {
            this.logger.LogInformation("TCP Endpoint stopped.");
        }
    }
}
