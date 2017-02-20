using Keeper.DotMudCore.CommandLoop.Parsing;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.CommandLoop.Internal
{
    public class CommandLoopMiddleware
        : IMiddleware
    {
        private readonly ICommandParser parser;
        private readonly SessionDelegate next;

        public CommandLoopMiddleware(ICommandParser parser, SessionDelegate next)
        {
            this.parser = parser;
            this.next = next;
        }

        public async Task Invoke(ISession session)
        {
            bool isQuitting = false;

            while (!isQuitting)
            {
                string prompt = "> ";

                await session.SendAsync(prompt);

                var commandLine = await session.ReceiveLineAsync();

                var info = this.parser.Parse(commandLine);

                if (info.IsValid)
                {
                    if (info.Name == "QUIT")
                    {
                        isQuitting = true;
                    }
                    else
                    {
                        await session.SendLineAsync($"Unknown command {info.Name}");
                    }
                }
            }
        }
    }
}
