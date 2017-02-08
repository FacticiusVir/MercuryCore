namespace Keeper.DotMudCore
{
    public struct LoginResult
    {
        public static LoginResult Success(string username, bool isNew)
        {
            return new LoginResult
            {
                Username = username,
                IsNew = isNew
            };
        }

        public static LoginResult Fail
        {
            get
            {
                return new LoginResult();
            }
        }

        public string Username
        {
            get;
            private set;
        }

        public bool IsSuccess
        {
            get;
            private set;
        }

        public bool IsNew
        {
            get;
            private set;
        }
    }
}