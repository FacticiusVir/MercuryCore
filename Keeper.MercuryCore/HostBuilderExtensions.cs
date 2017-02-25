using Keeper.MercuryCore.Internal;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Pipeline.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.MercuryCore
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder ConfigurePipeline(this IHostBuilder hostBuilder, Action<IServiceCollection<IPipeline>> pipelineAction)
            => hostBuilder.ConfigureServices(services => services.AddTransient<IPipelineFactory>(provider => new PipelineFactory(services, pipelineAction)));

        public static IHostBuilder ConfigureStartup(this IHostBuilder hostBuilder, Action<IServiceProvider> startupAction)
            => hostBuilder.ConfigureServices(services => services.AddSingleton<IHostStartup>(new DelegateHostStartup(startupAction)));
    }
}
