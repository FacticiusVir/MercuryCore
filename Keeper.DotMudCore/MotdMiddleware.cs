using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    internal class MotdMiddleware
        : IMiddleware
    {
        private readonly MotdOptions options;
        private readonly ILogger<MotdMiddleware> logger;
        private readonly SessionDelegate next;

        public MotdMiddleware(IOptions<MotdOptions> options, ILogger<MotdMiddleware> logger, SessionDelegate next)
        {
            this.options = options.Value;
            this.logger = logger;
            this.next = next;

            this.logger.LogInformation("Message of the Day configured as {Message}.", this.options.Message);
        }

        public async Task Invoke(ISession session)
        {
            this.logger.LogDebug("Displaying Message of the Day.");

            await session.SendLineAsync(options.Message);

            await this.next(session);
        }
    }
}
