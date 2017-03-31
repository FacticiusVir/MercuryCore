using Keeper.MercuryCore.CommandLoop.Parsing;
using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.CommandLoop.Internal
{
    public class CommandLoopMiddleware
        : IMiddleware
    {
        public Func<Task> BuildHandler(IServiceProvider serviceProvider, Func<Task> next)
        {
            var channel = serviceProvider.GetService<ITextChannel>();
            var parser = serviceProvider.GetService<ICommandParser>();

            return async () =>
            {
                bool isQuitting = false;

                while (!isQuitting)
                {
                    string prompt = "> ";

                    await channel.SendLineAsync(prompt);

                    var commandLine = await channel.ReceiveLineAsync();

                    var info = parser.Parse(commandLine);

                    if (info.IsValid)
                    {
                        if (info.Name == "QUIT")
                        {
                            isQuitting = true;
                        }
                        else
                        {
                            await channel.SendLineAsync($"Unknown command {info.Name}");
                        }
                    }
                }
            };
        }
    }
}
