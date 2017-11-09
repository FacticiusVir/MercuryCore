using Keeper.MercuryCore.CommandLoop;
using Keeper.MercuryCore.CommandLoop.Parsing;
using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.TestHarness
{
    public class LookCommandHandler
        : ICommandHandler
    {
        public string Name => "LOOK";

        public async Task Handle(ICommandLoop loop, CommandInfo info)
        {
            await loop.Provider.GetService<ITextChannel>().SendLineAsync("Looking");
        }
    }
}
