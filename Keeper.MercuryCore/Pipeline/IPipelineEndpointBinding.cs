using System;
using System.Collections.Generic;
using System.Text;

namespace Keeper.MercuryCore.Pipeline
{
    public interface IPipelineEndpointBinding
    {
        string EndpointName { get; }
    }
}
