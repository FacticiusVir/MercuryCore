using Keeper.MercuryCore;
using Keeper.MercuryCore.Internal;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Pipeline.Internal;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigurePipeline(this IHostBuilder hostBuilder, Action<IServiceCollection<IPipeline>> pipelineAction)
            => hostBuilder.ConfigureServices(services => services.AddTransient<IPipelineFactory>(provider => new PipelineFactory(services, pipelineAction)));

        public static IHostBuilder ConfigureStartup(this IHostBuilder hostBuilder, Action<IServiceProvider> startupAction)
            => hostBuilder.ConfigureServices(services => services.AddSingleton<IHostStartup>(new DelegateHostStartup(startupAction)));
    }
}
