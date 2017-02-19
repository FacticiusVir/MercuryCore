using Keeper.DotMudCore.Protocols;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using System.IO;

namespace Keeper.DotMudCore.ConsoleHost
{
    internal class Startup
        : StartupBase
    {
        private readonly IConfigurationRoot Configuration;

        static Startup()
        {
            Log.Logger = new LoggerConfiguration()

                                .CreateLogger();
        }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTcpEndpoint(this.Configuration.GetSection("tcp").Bind);

            services.AddSimpleLogin();

            services.AddMotd(options => options.Message = "Welcome to the DotMudCore test server!");
        }

        public override void Configure(IServerBuilder server)
        {
            server.UseSerilog(config => config
                                            .Enrich.FromLogContext()
                                            .WriteTo.ColoredConsole()
                                            .WriteTo.File(new JsonFormatter(), ".\\log.txt")
#if DEBUG
                                            .MinimumLevel.Verbose()
#endif
                                            );

            server.UseLinemodeTelnet();

            server.UseMotd();

            server.UseLogin();

            server.Run(async session =>
            {
                var identity = session.GetIdentityInfo();

                if (session.IsLinemodeTelnetSupported())
                {
                    var telnet = session.GetLinemodeTelnet();

                    await telnet.SendAsync("Hello, ");

                    await telnet.SendAsync(AnsiColour.Cyan, identity.Username);

                    await telnet.SendLineAsync();
                }
                else
                {
                    await session.SendLineAsync($"Hello, {identity.Username}");
                }

                await session.ReceiveLineAsync();
            });

            server.Services.GetService<Identity.IUserManager>().CreateUserAsync("TestUser", "test").Wait();
        }
    }
}
