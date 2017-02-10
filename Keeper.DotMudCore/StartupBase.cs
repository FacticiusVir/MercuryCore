using Microsoft.Extensions.DependencyInjection;

namespace Keeper.DotMudCore
{
    public abstract class StartupBase
        : IStartup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IServerBuilder server)
        {
        }
    }
}
