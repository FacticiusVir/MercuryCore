using Microsoft.Extensions.DependencyInjection;

namespace Keeper.DotMudCore
{
    public interface IStartup
    {
        void Configure(IServerBuilder server);

        void ConfigureServices(IServiceCollection services);
    }
}