namespace Keeper.MercuryCore.Identity
{
    public struct IdentityInfo
    {
        public IdentityInfo(string username, bool isNew)
            : this()
        {
            this.Username = username;
            this.IsNew = isNew;
        }

        public string Username
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
