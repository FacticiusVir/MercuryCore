using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Identity
{
    public class IdentityMiddleware
        : IMiddleware
    {
        private readonly SessionDelegate next;
        private readonly IIdentityManager identityManager;
        private readonly ILogger<IdentityMiddleware> logger;

        public IdentityMiddleware(SessionDelegate next, IIdentityManager identityManager, ILogger<IdentityMiddleware> logger)
        {
            this.next = next;
            this.identityManager = identityManager;
            this.logger = logger;
        }

        public async Task Invoke(ISession session)
        {
            var result = await this.identityManager.Authenticate(session);

            if (result.IsSuccess)
            {
                session.State.Set(new IdentityInfo(result.Username, result.Type == AuthenticateResultType.Registered));

                using (this.logger.BeginPropertyScope("Username", result.Username))
                {
                    await next(session);
                }

                session.State.Remove<IdentityInfo>();
            }
            else
            {
                this.logger.LogWarning("Authenticate failed: {AuthenticateResult}", result.Type);
            }
        }
    }
}
