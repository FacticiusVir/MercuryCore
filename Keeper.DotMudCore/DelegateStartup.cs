using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.DotMudCore
{
    public class DelegateStartup
        : IStartup
    {
        private readonly Action<IServerBuilder> configure;

        public DelegateStartup(Action<IServerBuilder> configure)
        {
            this.configure = configure;
        }

        public void Configure(IServerBuilder server)
        {
            this.configure(server);
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }
    }
}
