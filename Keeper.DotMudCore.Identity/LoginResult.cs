﻿namespace Keeper.DotMudCore.Identity
{
    public struct AuthenticateResult
    {
        public static AuthenticateResult Failed => new AuthenticateResult { Type = AuthenticateResultType.Failed };

        public static AuthenticateResult Success(string username, bool isNewRegistration = false)
            => new AuthenticateResult
                {
                    Type = isNewRegistration
                                ? AuthenticateResultType.Registered
                                : AuthenticateResultType.Authenticated,
                    Username = username
                };

        public AuthenticateResultType Type
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            private set;
        }

        public bool IsSuccess => this.Type == AuthenticateResultType.Authenticated || this.Type == AuthenticateResultType.Registered;
    }

    public enum AuthenticateResultType
    {
        Failed,
        Cancelled,
        Authenticated,
        Registered
    }
}