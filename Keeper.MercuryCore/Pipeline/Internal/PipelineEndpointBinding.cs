using System;
using System.Collections.Generic;
using System.Text;

namespace Keeper.MercuryCore.Pipeline.Internal
{
    public class PipelineEndpointBinding
        : IPipelineEndpointBinding
    {
        public PipelineEndpointBinding(string name)
        {
            this.EndpointName = name;
        }

        public string EndpointName
        {
            get;
        }
    }
}
