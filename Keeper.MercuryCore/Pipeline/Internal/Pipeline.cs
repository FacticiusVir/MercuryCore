using Keeper.MercuryCore.Channel;
using Keeper.MercuryCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Pipeline.Internal
{
    internal class Pipeline
        : IPipeline
    {
        private readonly ILogger<Pipeline> logger;
        private readonly IServiceCollection services;
        private readonly IServiceProvider serviceProvider;
        private readonly int pipelineId;
        private readonly IEnumerable<IEndpoint> endpoints;
        private readonly IEnumerable<IMiddleware> middlewares;

        public Pipeline(ILogger<Pipeline> logger, IServiceCollection pipelineServices, int pipelineId)
        {
            this.logger = logger;
            this.services = pipelineServices;
            this.serviceProvider = pipelineServices.BuildServiceProvider();
            this.pipelineId = pipelineId;

            this.endpoints = this.serviceProvider.GetServices<IEndpoint>();

            this.middlewares = this.serviceProvider.GetServices<IMiddleware>();

            foreach (var endpoint in this.endpoints)
            {
                endpoint.NewConnection += this.OnNewConnection;
            }
        }

        private async Task OnNewConnection(IConnection connection)
        {
            using (logger.BeginPropertyScope("Connection", connection.UniqueIdentifier))
            {
                logger.LogInformation("New connection: {Connection}");

                var sessionServices = new ChildServiceCollection<ISession>(this.services, this.serviceProvider);

                var channel = new Lazy<AsciiChannel>();

                sessionServices.AddSingleton<IChannel>(provider => channel.Value);
                sessionServices.AddSingleton<ITextChannel>(provider => channel.Value);

                var sessionServiceProvider = sessionServices.BuildServiceProvider();

                sessionServiceProvider.GetService<IChannel>().Bind(connection);

                Func<Task> pipeline = () => Task.CompletedTask;

                foreach (var middleware in this.middlewares.Reverse())
                {
                    pipeline = middleware.BuildHandler(sessionServiceProvider, pipeline);
                }

                var nullBlock = new ActionBlock<ArraySegment<byte>>(data => { });

                using (connection.Receive.LinkTo(nullBlock))
                {
                    await pipeline();

                    await Task.Delay(2500);
                }

                connection.Close();

                await connection.Closed;

                logger.LogInformation("Connection closed: {Connection}");
            }
        }

        public void Start()
        {
            foreach (var endpoint in this.endpoints)
            {
                endpoint.Start();
            }

            this.logger.LogInformation("Pipeline {PipelineId} started.", this.pipelineId);
        }

        public void Stop()
        {
            foreach (var endpoint in this.endpoints)
            {
                endpoint.Stop();
            }

            this.logger.LogInformation("Pipeline {PipelineId} stopped.", this.pipelineId);
        }
    }
}