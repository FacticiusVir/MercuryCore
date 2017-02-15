namespace Keeper.DotMudCore
{
    internal class Session
        : ISession
    {
        public IConnection Connection
        {
            get;
            private set;
        }

        public Session(IConnection connection)
        {
            this.Connection = connection;
        }

        public ISessionStateManager State
        {
            get;
            private set;
        } = new SessionStateManager();
    }
}
