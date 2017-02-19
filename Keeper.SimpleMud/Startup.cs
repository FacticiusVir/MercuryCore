using Keeper.DotMudCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Json;
using System.IO;

namespace Keeper.SimpleMud
{
    internal class Startup
        : IStartup
    {
        public Startup()
        {
            this.Configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json")
                                        .Build();
        }

        public IConfigurationRoot Configuration
        {
            get;
            private set;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTcpEndpoint(this.Configuration.GetSection("tcp").Bind);

            services.AddMotd(options => options.Message = "Welcome to SimpleMUD!\n");
        }

        public void Configure(IServerBuilder server)
        {
            server.UseSerilog(config => config
                                        .Enrich.FromLogContext()
                                        .WriteTo.LiterateConsole()
                                        .WriteTo.File(new JsonFormatter(), ".\\log.txt")
#if DEBUG
                                        .MinimumLevel.Verbose()
#endif
                                        );

            server.UseMotd();

            server.Run(async session =>
            {
                await session.ReceiveLineAsync();
            });
        }
    }
}