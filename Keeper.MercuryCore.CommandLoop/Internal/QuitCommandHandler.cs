using Keeper.MercuryCore.CommandLoop.Parsing;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.CommandLoop.Internal
{
    public class QuitCommandHandler
        : ICommandHandler
    {
        public string Name => "QUIT";

        public Task Handle(ICommandLoop loop, CommandInfo info)
        {
            loop.IsRunning = false;

            return Task.CompletedTask;
        }
    }
}
