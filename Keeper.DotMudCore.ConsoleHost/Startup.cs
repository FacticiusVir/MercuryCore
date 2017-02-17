using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using System.IO;
using System.Linq;

namespace Keeper.DotMudCore
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

            server.Run(async session =>
            {
                await session.Connection.SendAsync(new byte[] { 0xff, 0xfd, 0x22, 0xff, 0xfb, 0x01, 0xff, 0xfb, 0x03 });
                
                byte[] data = new byte[255];

                await session.Connection.ReceiveAsync(data, 0, 9);

                await session.Connection.SendAsync("\x1b[10;32HOption One");
                await session.Connection.SendAsync("\x1b[15;32HOption Two");
                await session.Connection.SendAsync("\x1b[20;32HOption Three");
                await session.Connection.SendAsync("\x1b[?25h\x1b[H");
                
                int selection = 0;

                System.Array.Clear(data, 0, data.Length);

                while (!data.Any(x => x == (byte)'\n'))
                {
                    string update = "";

                    for (int index = 0; index < 3; index++)
                    {
                        update += $"\x1b[{10 + 5 * index};30H";
                        update += index == selection ? "> " : "  ";
                    }
                    update += "\x1b[H";

                    await session.Connection.SendAsync(update);

                    int count = await session.Connection.ReceiveAsync(data);

                    if (count == 3 && data[0] == 27 && data[1] == 91)
                    {
                        switch (data[2])
                        {
                            case 65:
                                selection--;

                                if (selection < 0)
                                {
                                    selection = 2;
                                }
                                break;
                            case 66:
                                selection++;

                                if (selection > 2)
                                {
                                    selection = 0;
                                }
                                break;
                        }
                    }
                }
            });
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTcpEndpoint(this.Configuration.GetSection("tcp").Bind);

            services.AddSimpleLogin();
        }
    }
}
