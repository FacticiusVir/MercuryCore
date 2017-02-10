using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class SimpleLoginManager
        : ILoginManager
    {
        private readonly ILogger<SimpleLoginManager> logger;

        public SimpleLoginManager(ILogger<SimpleLoginManager> logger)
        {
            this.logger = logger;
        }

        public async Task<LoginResult> Login(IConnection connection)
        {
            this.logger.LogDebug("Logging in");

            string username = null;
            bool isUsernameValid = false;

            while (!isUsernameValid)
            {
                await connection.SendAsync("Please enter your username:");

                this.logger.LogDebug("Receiving username");

                username = await connection.ReceiveAsync();

                if (username == null)
                {
                    this.logger.LogDebug("Client disconnected");

                    return LoginResult.Disconnected;
                }
                else
                {
                    this.logger.LogDebug("Username received: {SubmittedUsername}", username);
                }

                isUsernameValid = !string.IsNullOrWhiteSpace(username);

                if (!isUsernameValid)
                {
                    this.logger.LogDebug("Submitted username not valid");

                    await connection.SendAsync("Invalid username.");
                }
                else
                {
                    this.logger.LogDebug("Username accepted");
                }
            }

            return LoginResult.Success(username);
        }
    }
}
