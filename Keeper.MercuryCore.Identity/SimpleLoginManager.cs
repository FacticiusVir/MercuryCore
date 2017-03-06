using Keeper.MercuryCore;
using Keeper.MercuryCore.Session;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Keeper.MercuryCore.Identity
{
    public class SimpleLoginManager
        : IIdentityManager
    {
        private readonly ILogger<SimpleLoginManager> logger;
        private readonly IUserManager userManager;

        public SimpleLoginManager(ILogger<SimpleLoginManager> logger, IUserManager userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }

        public async Task<AuthenticateResult> Authenticate(ITextChannel channel)
        {
            this.logger.LogDebug("Logging in");

            string username = null;
            bool isUsernameValid = false;

            while (!isUsernameValid)
            {
                await channel.SendLineAsync("Please enter your username:");

                this.logger.LogDebug("Receiving username");

                username = await channel.ReceiveLineAsync();

                this.logger.LogDebug("Username received: {SubmittedUsername}", username);

                isUsernameValid = !string.IsNullOrWhiteSpace(username);

                if (!isUsernameValid)
                {
                    this.logger.LogDebug("Submitted username not valid");

                    await channel.SendLineAsync("Invalid username");
                }
                else
                {
                    this.logger.LogDebug("Username accepted");

                    if (await this.userManager.CheckUserAsync(username))
                    {
                        this.logger.LogDebug("User found");

                        await channel.SendLineAsync("Please enter password");

                        string password = await channel.ReceiveLineAsync();

                        if (await this.userManager.CheckUserAsync(username, password))
                        {
                            this.logger.LogInformation("{Username} logged in successfully", username);

                            return AuthenticateResult.Success(username);
                        }
                        else
                        {
                            await channel.SendLineAsync("Password incorrect");

                            this.logger.LogWarning("{Username} login failed", username);

                            isUsernameValid = false;
                        }
                    }
                    else
                    {
                        this.logger.LogDebug("No existing user found");

                        await channel.SendLineAsync("Username not found - please enter a password to register, or blank to re-enter username");

                        bool isPasswordValid = false;

                        string password = null;

                        while (!isPasswordValid && isUsernameValid)
                        {
                            password = await channel.ReceiveLineAsync();

                            this.logger.LogDebug("Password received");

                            if (password == "")
                            {
                                isUsernameValid = false;
                            }
                            else
                            {
                                await channel.SendLineAsync("Please confirm password");

                                string passwordConfirmation = await channel.ReceiveLineAsync();

                                this.logger.LogDebug("Password confirmation received");

                                isPasswordValid = password.Equals(passwordConfirmation);

                                if (isPasswordValid)
                                {
                                    this.logger.LogDebug("Password accepted");
                                }
                                else
                                {
                                    await channel.SendLineAsync("Passwords do not match, please re-enter password");
                                    this.logger.LogDebug("Passwords do not match");
                                }
                            }
                        }

                        if (isPasswordValid)
                        {
                            await this.userManager.CreateUserAsync(username, password);

                            this.logger.LogInformation("New user {Username} registered", username);
                        }
                    }
                }
            }

            return AuthenticateResult.Success(username);
        }
    }
}
