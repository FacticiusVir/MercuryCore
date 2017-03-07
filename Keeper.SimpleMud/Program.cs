using Keeper.MercuryCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Serilog;
using Keeper.MercuryCore.Session;

namespace Keeper.SimpleMud
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json")
                                        .Build();

            var host = new HostBuilder()
                                    .ConfigureSerilog(config => config
                                                                .Enrich.FromLogContext()
                                                                .WriteTo.LiterateConsole())
                                    .ConfigurePipeline(pipeline =>
                                    {
                                        pipeline.AddTcpEndpoint(configuration.GetSection("tcp").Bind);

                                        pipeline.UseTelnetChannel();

                                        pipeline.UseMotd(options => options.Message = "Welcome to SimpleMUD!");
                                    })
                                    .Build();

            host.Run();
        }
    }
}