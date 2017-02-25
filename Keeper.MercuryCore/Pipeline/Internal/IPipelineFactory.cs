using System;

namespace Keeper.MercuryCore.Pipeline.Internal
{
    internal interface IPipelineFactory
    {
        IPipeline Create(IServiceProvider hostServiceProvider, int pipelineId);
    }
}
