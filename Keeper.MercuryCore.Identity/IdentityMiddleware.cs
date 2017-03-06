using Keeper.MercuryCore.Pipeline;
using Keeper.MercuryCore.Session;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Identity
{
    public class IdentityMiddleware
        : IMiddleware
    {
        public IdentityMiddleware()
        {
        }

        public Func<Task> BuildHandler(IServiceProvider serviceProvider, Func<Task> next)
        {
            var identityManager = serviceProvider.GetService<IIdentityManager>();
            var logger = serviceProvider.GetService<ILogger<IdentityMiddleware>>();
            var channel = serviceProvider.GetService<ITextChannel>();
            var sessionState = serviceProvider.GetService<IStateManager>();

            return async () =>
            {
                var result = await identityManager.Authenticate(channel);

                if (result.IsSuccess)
                {
                    sessionState.Set(new IdentityInfo(result.Username, result.Type == AuthenticateResultType.Registered));

                    using (logger.BeginPropertyScope("Username", result.Username))
                    {
                        await next();
                    }

                    sessionState.Remove<IdentityInfo>();
                }
                else
                {
                    logger.LogWarning("Authenticate failed: {AuthenticateResult}", result.Type);
                }
            };
        }
    }
}
