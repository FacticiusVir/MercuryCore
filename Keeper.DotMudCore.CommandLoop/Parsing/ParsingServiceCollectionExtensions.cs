using Keeper.DotMudCore.CommandLoop;
using Keeper.DotMudCore.CommandLoop.Parsing;
using Keeper.DotMudCore.CommandLoop.Parsing.Internal;

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
