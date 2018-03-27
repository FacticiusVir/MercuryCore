using Keeper.MercuryCore.Pipeline;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.WebSockets
{
    public class WebSocketEndpoint
        : IEndpoint
    {
        private readonly ILogger<WebSocketEndpoint> logger;
        private IWebHost host;

        public WebSocketEndpoint(ILogger<WebSocketEndpoint> logger, string name)
        {
            this.logger = logger;
            this.Name = name;

            this.host = WebHost.CreateDefaultBuilder()
                                .ConfigureServices(services => services.AddSingleton(this))
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
