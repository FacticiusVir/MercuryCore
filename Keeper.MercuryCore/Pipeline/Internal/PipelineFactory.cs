using Keeper.MercuryCore.Util;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.MercuryCore.Pipeline.Internal
{
    public class PipelineFactory
        : IPipelineFactory
    {
        private IServiceCollection hostServices;
        private Action<IServiceCollection<IPipeline>> pipelineAction;

        public PipelineFactory(IServiceCollection hostServices, Action<IServiceCollection<IPipeline>> pipelineAction)
        {
            this.hostServices = hostServices;
            this.pipelineAction = pipelineAction;
        }

        public IPipeline Create(IServiceProvider hostServiceProvider, int pipelineId)
        {
            var pipelineServices = new ChildServiceCollection<IPipeline>(this.hostServices, hostServiceProvider);

            this.pipelineAction(pipelineServices);

            return ActivatorUtilities.CreateInstance<Pipeline>(hostServiceProvider, pipelineServices, pipelineId);
        }
    }
}
