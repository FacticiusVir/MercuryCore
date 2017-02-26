using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace Keeper.MercuryCore.Pipeline.Internal
{
    internal class Pipeline
        : IPipeline
    {
        private readonly ILogger<Pipeline> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly int pipelineId;
        private readonly IEnumerable<IEndpoint> endpoints;

        public Pipeline(ILogger<Pipeline> logger, IServiceCollection pipelineServices, int pipelineId)
        {
            this.logger = logger;
            this.serviceProvider = pipelineServices.BuildServiceProvider();
            this.pipelineId = pipelineId;

            this.endpoints = this.serviceProvider.GetServices<IEndpoint>();

            foreach (var endpoint in this.endpoints)
            {
                endpoint.NewConnection += this.OnNewConnection;
            }
        }

        private async void OnNewConnection(IConnection connection)
        {
            using (logger.BeginPropertyScope("Connection", connection.UniqueIdentifier))
            {
                logger.LogInformation("New Connection: {Connection}");

                var sendData = Encoding.ASCII.GetBytes("Test\r\n");

                await connection.Send.SendAsync(new ArraySegment<byte>(sendData));

                bool isRunning = true;

                while (isRunning)
                {
                    var data = await connection.Receive.ReceiveAsync();

                    Console.WriteLine(data.Count);

                    if (data.Count == 2)
                    {
                        isRunning = false;
                    }
                }

                connection.Close();

                await connection.Closed;
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