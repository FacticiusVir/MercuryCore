using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols.Internal
{
    public class LinemodeTelnetMiddleware
        : IMiddleware
    {
        private readonly ILogger<LinemodeTelnetMiddleware> logger;
        private readonly SessionDelegate next;

        public LinemodeTelnetMiddleware(ILogger<LinemodeTelnetMiddleware> logger, SessionDelegate next)
        {
            this.logger = logger;
            this.next = next;

            this.logger.LogInformation("Linemode Telnet middleware enabled.");
        }

        public async Task Invoke(ISession session)
        {
            this.logger.LogDebug("Enabling Linemode Telnet.");

            session.Protocol.MarkSupport<ILinemodeTelnet, LinemodeTelnet>();
            await session.Protocol.Get<ILinemodeTelnet>().MakeActiveAsync();

            await this.next(session);
        }
    }
}
