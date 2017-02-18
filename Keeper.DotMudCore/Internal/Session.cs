using Keeper.DotMudCore.Protocols;
using Keeper.DotMudCore.Protocols.Internal;
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

            var protocol = ActivatorUtilities.CreateInstance<ProtocolManager>(provider, connection);

            protocol.MarkSupport<IPlainAscii, PlainAscii>();
            protocol.MakeActiveAsync<IPlainAscii>().Wait();

            this.Protocol = protocol;
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
