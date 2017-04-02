using Keeper.MercuryCore;
using Keeper.MercuryCore.CommandLoop;
using Keeper.MercuryCore.CommandLoop.Parsing;
using Keeper.MercuryCore.CommandLoop.Parsing.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ParsingServiceCollectionExtensions
    {
        public static IServiceCollection<ICommandLoop> AddVerbObjectParser(this IServiceCollection<ICommandLoop> services)
        {
            services.AddSingleton<ICommandParser, VerbObjectParser>();

            return services;
        }
    }
}
