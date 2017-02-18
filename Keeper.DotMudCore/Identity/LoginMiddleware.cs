using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Identity
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
            var result = await this.loginManager.Login(session);

            if (result.IsSuccess)
            {
                session.State.Set(new IdentityInfo(result.Username, result.Type == LoginResultType.Registered));

                using (this.logger.BeginPropertyScope("Username", result.Username))
                {
                    await next(session);
                }
            }
            else
            {
                this.logger.LogWarning("Login failed: {LoginResult}", result.Type);
            }
        }
    }
}
