using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols.Internal
{
    public class TelnetMiddleware
        : IMiddleware
    {
        private readonly ILogger<TelnetMiddleware> logger;
        private readonly SessionDelegate next;

        public TelnetMiddleware(ILogger<TelnetMiddleware> logger, SessionDelegate next)
        {
            this.logger = logger;
            this.next = next;

            this.logger.LogInformation("Telnet middleware enabled.");
        }

        public async Task Invoke(ISession session)
        {
            this.logger.LogDebug("Enabling Telnet.");

            session.Protocol.MarkSupport<ITelnet, Telnet>();
            await session.Protocol.Get<ITelnet>().MakeActiveAsync();

            await this.next(session);
        }
    }
}
