using Keeper.MercuryCore.CommandLoop;
using Keeper.MercuryCore.CommandLoop.Parsing;
using Keeper.MercuryCore.CommandLoop.Parsing.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ParsingServiceCollectionExtensions
    {
        public static ICommandLoopServiceCollection AddVerbObjectParser(this ICommandLoopServiceCollection services)
        {
            return services.AddSingleton<ICommandParser, VerbObjectParser>();
        }
    }
}
