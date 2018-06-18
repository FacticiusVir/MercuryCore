using Keeper.MercuryCore.Session;
using Keeper.MercuryCore.Session.Internal;
using Keeper.MercuryCore.Util;
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
            using (logger.BeginPropertyScope("PipelineId", this.pipelineId))
            {
                logger.LogInformation("New connection: {Connection} on pipeline {PipelineId}");

                try
                {
                    var sessionServices = new ChildServiceCollection<ISession>(this.services, this.serviceProvider);

                    sessionServices.AddSingleton<IStateManager, StateManager>();

                    var sessionServiceProvider = sessionServices.BuildServiceProvider();

                    var channels = sessionServiceProvider.GetServices<IChannel>();

                    Action<byte> stack = x => { };
                    Action<SignalType> signalStack = x => { };

                    Func<ArraySegment<byte>, Task> send = connection.Send.SendAsync;

                    foreach (var channel in channels.Reverse())
                    {
                        var stackTemp = stack;
                        var signalTemp = signalStack;

                        stack = datum => channel.Handle(datum, stackTemp, signalTemp);
                        signalStack = signal => channel.Signal(signal, signalTemp);
                    }

                    foreach(var channel in channels)
                    {
                        send = channel.Bind(send);
                    }

                    connection.Receive.LinkTo(new ActionBlock<ArraySegment<byte>>(data =>
                    {
                        foreach (var datum in data)
                        {
                            stack(datum);
                        }

                        signalStack(SignalType.EndOfFrame);
                    }));

                    Func<Task> pipeline = () => Task.CompletedTask;

                    foreach (var middleware in this.middlewares.Reverse())
                    {
                        pipeline = middleware.BuildHandler(sessionServiceProvider, pipeline);
                    }

                    logger.LogDebug("Pipeline {PipelineId} built; starting.");

                    await pipeline();

                    signalStack(SignalType.ConnectionClosed);
                }
                catch (ClientDisconnectedException)
                {
                    this.logger.LogWarning("Client disconnected unexpectedly: {Connection}");
                }
                finally
                {
                    connection.Close();

                    await connection.Closed;

                    logger.LogInformation("Connection closed: {Connection}");
                }
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