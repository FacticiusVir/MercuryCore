using Microsoft.Extensions.DependencyInjection;
using System;

namespace Keeper.DotMudCore.Internal
{
    internal class Session
        : ISession
    {
        public IConnection Connection
        {
            get;
            private set;
        }
        
        public Session(IServiceProvider provider, IConnection connection)
        {
            this.Connection = connection;
            this.Protocol = new ProtocolManager(provider, connection);
        }

        public ISessionStateManager State
        {
            get;
            private set;
        } = new SessionStateManager();

        public IProtocolManager Protocol
        {
            get;
            private set;
        }
    }
}
