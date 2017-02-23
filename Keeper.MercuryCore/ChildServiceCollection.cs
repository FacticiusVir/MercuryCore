using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Keeper.MercuryCore
{
    internal class ChildServiceCollection
        : List<ServiceDescriptor>, IServiceCollection
    {
        public ChildServiceCollection(IServiceCollection parent)
        {

        }
    }
}
