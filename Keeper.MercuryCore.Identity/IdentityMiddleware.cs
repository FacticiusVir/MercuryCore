using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Identity
{
    public class IdentityMiddleware<T>
        : IMiddleware
        where T : IIdentityManager
    {
        public Func<Task> BuildHandler(IServiceProvider serviceProvider, Func<Task> next)
        {
            var identityManager = serviceProvider.GetService<T>();
            var logger = serviceProvider.GetService<ILogger<T>>();
            var sessionState = serviceProvider.GetService<IStateManager>();
            var persistance = serviceProvider.GetServices<IIdentityPersistance>();

            return async () =>
            {
                if (!sessionState.TryGetIdentityInfo(out var info))
                {
                    var result = await identityManager.AuthenticateAsync(serviceProvider);

                    if (result.IsSuccess)
                    {
                        sessionState.Set(new IdentityInfo(result.Username, result.Type == AuthenticateResultType.Registered));

                        foreach (var manager in persistance)
                        {
                            await manager.PersistAsync(serviceProvider);
                        }

                        using (logger.BeginPropertyScope("Username", result.Username))
                        {
                            await next();
                        }

                        sessionState.Remove<IdentityInfo>();
                    }
                    else
                    {
                        if (result.Type == AuthenticateResultType.Cancelled)
                        {
                            logger.LogInformation("Authenticate cancelled.");
                        }
                        else
                        {
                            logger.LogWarning("Authenticate failed: {AuthenticateResult}", result.Type);
                        }

                        await next();
                    }
                }
                else
                {
                    logger.LogInformation($"{typeof(T).Name} Authentication skipped.");

                    await next();
                }
            };
        }
    }
}
