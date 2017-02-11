using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using System.IO;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.ConsoleHost
{
    internal class Startup
        : StartupBase
    {
        private readonly IConfigurationRoot Configuration;

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public override void Configure(IServerBuilder server)
        {
            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .WriteTo.ColoredConsole()
                            .WriteTo.File(new JsonFormatter(), ".\\log.txt")
#if DEBUG
                                                            .MinimumLevel.Debug()
#endif
                                                            .CreateLogger();

            server.Services.GetService<ILoggerFactory>().AddSerilog();

            server.UseLogin();

            server.Run(async session =>
            {
                string username = session.GetState<IdentityInfo>().Username;

                await session.Connection.SendAsync($"Hello {username}!");

                await Task.Delay(2000);
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<TcpOptions>(this.Configuration.GetSection("tcp"));

            services.AddSingleton<ILoginManager, SimpleLoginManager>();
        }
    }
}
