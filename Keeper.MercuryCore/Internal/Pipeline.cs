using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keeper.MercuryCore.Internal
{
    internal class Pipeline
        : IPipeline
    {
        private readonly ILogger<Pipeline> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly int pipelineId;

        public Pipeline(ILogger<Pipeline> logger, IServiceCollection pipelineServices, int pipelineId)
        {
            this.logger = logger;
            this.serviceProvider = pipelineServices.BuildServiceProvider();
            this.pipelineId = pipelineId;
        }

        public void Start()
        {
            this.logger.LogInformation("Pipeline {PipelineId} started.", this.pipelineId);
        }

        public void Stop()
        {
            this.logger.LogInformation("Pipeline {PipelineId} stopped.", this.pipelineId);
        }
    }
}