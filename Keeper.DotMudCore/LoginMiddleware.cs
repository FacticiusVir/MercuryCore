using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class LoginMiddleware
        : IMiddleware
    {
        private readonly SessionDelegate next;
        private readonly ILoginManager loginManager;
        private readonly ILogger<LoginMiddleware> logger;

        public LoginMiddleware(SessionDelegate next, ILoginManager loginManager, ILogger<LoginMiddleware> logger)
        {
            this.next = next;
            this.loginManager = loginManager;
            this.logger = logger;
        }

        public async Task Invoke(ISession session)
        {
            var result = await this.loginManager.Login(session.Connection);

            if (result.IsSuccess)
            {
                session.SetState(new IdentityInfo(result.Username, result.Type == LoginResultType.Registered));

                await next(session);
            }
            else
            {
                this.logger.LogWarning("Login failed: {LoginResult}", result.Type);
            }
        }
    }
}
