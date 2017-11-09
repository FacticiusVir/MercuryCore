using Keeper.MercuryCore.CommandLoop.Parsing;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.CommandLoop.Internal
{
    public class CommandLoopMiddleware
        : IMiddleware
    {
        private class LoopInstance
            : ICommandLoop
        {
            public bool IsRunning
            {
                get;
                set;
            }

            public IServiceProvider Provider
            {
                get;
                set;
            }
        }

        public Func<Task> BuildHandler(IServiceProvider serviceProvider, Func<Task> next)
        {
            var channel = serviceProvider.GetRequiredService<ITextChannel>();
            var parser = serviceProvider.GetRequiredService<ICommandParser>();
            var handlerLookup = serviceProvider.GetServices<ICommandHandler>().ToDictionary(x => x.Name.ToUpperInvariant());

            return async () =>
            {
                var loopInstance = new LoopInstance
                {
                    IsRunning = true,
                    Provider = serviceProvider
                };
                
                while (loopInstance.IsRunning)
                {
                    var commandLine = await channel.ReceiveLineAsync();

                    var info = parser.Parse(commandLine);

                    if (info.IsValid)
                    {
                        if (handlerLookup.TryGetValue(info.Name, out var handler))
                        {
                            await handler.Handle(loopInstance, info);
                        }
                        else
                        {
                            await channel.SendLineAsync($"Unknown command {info.Name}");
                        }
                    }
                    else
                    {
                        await channel.SendLineAsync("Invalid command");
                    }
                }
            };
        }
    }
}
