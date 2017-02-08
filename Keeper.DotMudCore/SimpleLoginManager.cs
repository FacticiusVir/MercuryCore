using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Keeper.DotMudCore
{
    public class SimpleLoginManager
        : ILoginManager
    {
        public async Task<LoginResult> Login(IConnection connection)
        {
            string username = null;
            bool isUsernameValid = false;

            while (!isUsernameValid)
            {
                await connection.SendAsync("Please enter your username:");

                username = await connection.ReceiveAsync();

                if (username == null)
                {
                    return LoginResult.Fail;
                }

                isUsernameValid = !string.IsNullOrWhiteSpace(username);

                if (!isUsernameValid)
                {
                    await connection.SendAsync("Invalid username.");
                }
            }

            return LoginResult.Success(username, true);
        }
    }
}
