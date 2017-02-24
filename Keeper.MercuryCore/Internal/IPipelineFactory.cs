using System;

namespace Keeper.MercuryCore.Internal
{
    internal interface IPipelineFactory
    {
        IPipeline Create(IServiceProvider hostServiceProvider, int pipelineId);
    }
}
