using System;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class Server
    {
        private readonly ILogger<Server> logger;
        private readonly IEndpoint endpoint;
        private readonly ILoginManager loginManager;

        public Server(ILoggerFactory loggerFactory,
                        IEndpoint endpoint,
                        ILoginManager loginManager)
        {
            this.logger = loggerFactory.CreateLogger<Server>();
            this.endpoint = endpoint;
            this.loginManager = loginManager;

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

                    await conn.SendAsync("Welcome to the demo server");

                    var loginResult = await this.loginManager.Login(conn);

                    if (!loginResult.IsSuccess)
                    {
                        this.logger.LogWarning("Login failed: {Reason}", loginResult.Type);

                        conn.Close();

                        return;
                    }

                    using (this.logger.BeginPropertyScope("User", loginResult.Username))
                    {
                        this.logger.LogInformation("{User} logged in.");

                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    this.logger.LogError(0, ex, "Exception thrown while processing connection");
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
