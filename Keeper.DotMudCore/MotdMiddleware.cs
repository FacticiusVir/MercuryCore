using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    internal class MotdMiddleware
        : IMiddleware
    {
        private readonly MotdOptions options;
        private readonly SessionDelegate next;

        public MotdMiddleware(IOptions<MotdOptions> options, SessionDelegate next)
        {
            this.options = options.Value;
            this.next = next;
        }

        public async Task Invoke(ISession session)
        {
            await session.Connection.SendLineAsync(options.Message);

            await this.next(session);
        }
    }
}
