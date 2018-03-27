using Keeper.MercuryCore.Pipeline;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.WebSockets
{
    public class WebSocketEndpoint
        : IEndpoint
    {
        private readonly ILogger<WebSocketEndpoint> logger;
        private IWebHost host;

        public WebSocketEndpoint(IOptionsFactory<WebSocketOptions> optionsFactory, ILogger<WebSocketEndpoint> logger, ILoggerFactory loggerFactory, string name)
        {
            this.logger = logger;
            this.Name = name;

            var options = optionsFactory.Create(name);

            int port = options.Port ?? 80;

            this.logger.LogInformation("WebSocket Endpoint configured on port {Port}", port);

            this.host = WebHost.CreateDefaultBuilder()
                                .ConfigureServices(services => services.AddSingleton(this))
                                .ConfigureLogging(logging =>
                                {
                                    logging.ClearProviders();
                                    logging.AddProvider(new LoggerProvider(loggerFactory));
                                })
                                .UseKestrel(kestrelOptions =>
                                {
                                    kestrelOptions.Listen(options.Address, port);
                                })
                                .UseStartup<Startup>()
                                .Build();
        }

        public string Name
        {
            get;
        }

        public event Func<IConnection, Task> NewConnection;

        public void Start()
        {
            this.host.StartAsync();
        }

        public void Stop()
        {
            this.host.StopAsync();
        }

        private class LoggerProvider
            : ILoggerProvider
        {
            private readonly ILoggerFactory factory;

            public LoggerProvider(ILoggerFactory factory)
            {
                this.factory = factory;
            }

            public ILogger CreateLogger(string categoryName) => this.factory.CreateLogger(categoryName);

            public void Dispose() => this.factory.Dispose();
        }

        private class Startup
        {
            public Startup(WebSocketEndpoint endpoint)
            {
                this.Endpoint = endpoint;
            }

            public WebSocketEndpoint Endpoint
            {
                get;
            }

            public void ConfigureServices(IServiceCollection services)
            {
            }

            public void Configure(IApplicationBuilder app, IHostingEnvironment env)
            {
                app.UseWebSockets();
                app.Use(async (http, next) =>
                {
                    if (http.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await http.WebSockets.AcceptWebSocketAsync();

                        await this.Endpoint?.NewConnection(new WebSocketConnection(this.Endpoint.Name, webSocket));
                    }
                    else
                    {
                        await next();
                    }
                });
            }
        }
    }
}
