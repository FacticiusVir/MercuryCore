using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Identity
{
    public class InMemoryUserManager
        : IUserManager
    {
        private struct UserInfo
        {
            public string Username
            {
                get;
                set;
            }

            public string Password
            {
                get;
                set;
            }
        }

        private Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

        private readonly object userLock = new object();

        public Task<bool> CheckUserAsync(string username, string password = null)
        {
            return Task.Run(() =>
            {
                lock (userLock)
                {
                    string key = Normalise(username);

                    UserInfo info;

                    if (!this.users.TryGetValue(key, out info))
                    {
                        return false;
                    }
                    else
                    {
                        return password == null
                                    || password == info.Password;
                    }
                }
            });
        }

        public Task<bool> CreateUserAsync(string username, string password)
        {
            return Task.Run(() =>
            {
                lock (this.userLock)
                {
                    string key = Normalise(username);

                    if (this.users.ContainsKey(key))
                    {
                        return false;
                    }
                    else
                    {
                        this.users.Add(key, new UserInfo
                        {
                            Username = username,
                            Password = password
                        });

                        return true;
                    }
                }
            });
        }

        private static string Normalise(string username)
        {
            return username.ToUpperInvariant();
        }
    }
}
