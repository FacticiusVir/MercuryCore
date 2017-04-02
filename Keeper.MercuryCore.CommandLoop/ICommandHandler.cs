using Keeper.MercuryCore.CommandLoop.Parsing;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.CommandLoop
{
    public interface ICommandHandler
    {
        string Name
        {
            get;
        }

        Task Handle(ICommandLoop loop, CommandInfo info);
    }
}
