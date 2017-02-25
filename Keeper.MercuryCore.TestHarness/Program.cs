using Serilog;

namespace Keeper.MercuryCore.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new HostBuilder()
                            .ConfigureSerilog(config => config.WriteTo.LiterateConsole())
                            .ConfigurePipeline(pipeline =>
                            {
                                pipeline.AddTcpEndpoint(options => options.Port = 5000);
                            })
                            .Build();

            host.Run();
        }
    }
}