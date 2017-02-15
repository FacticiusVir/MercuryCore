using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Internal
{
    internal class Server
    {
        private readonly ILogger<Server> logger;
        private readonly IEndpoint endpoint;
        private readonly SessionDelegate app;
        private readonly IServiceProvider provider;

        public Server(ILogger<Server> logger,
                        IEnumerable<IEndpoint> endpoint,
                        IServerBuilder builder,
                        IServiceProvider provider)
        {
            this.logger = logger;
            this.endpoint = endpoint.First();
            this.app = builder.Build();
            this.provider = provider;

            this.endpoint.NewConnection += conn => Task.Run(() => this.HandleConnection(conn));

            this.logger.LogDebug("Created {SourceContext}");
        }

        private async Task HandleConnection(IConnection conn)
        {
            using (this.logger.BeginPropertyScope("Connection", conn.UniqueIdentifier))
            {
                try
                {
                    this.logger.LogInformation("New connection: {Connection}");

                    var session = new Session(this.provider, conn);

                    await this.app(session);
                }
                catch(ClientDisconnectedException)
                {
                    this.logger.LogWarning("Client disconnected unexpectedly");
                }
                catch (Exception ex)
                {
                    this.logger.LogError(0, ex, "Exception thrown while processing connection");
                }
                finally
                {
                    this.logger.LogInformation("Connection closed: {Connection}");

                    conn.Close();
                }
            }
        }

        public void Startup()
        {
            this.logger.LogDebug("{SourceContext} starting");
            this.endpoint.Start();
            this.logger.LogInformation("{SourceContext} started");
        }

        public void Shutdown()
        {
            this.logger.LogDebug("{SourceContext} stopping");
            this.endpoint.Stop();
            this.logger.LogInformation("{SourceContext} stopped");
        }
    }
}
