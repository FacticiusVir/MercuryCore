using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols.Internal
{
    internal class ProtocolManager
        : IProtocolManager, IProtocolManagerControl
    {
        private readonly IServiceProvider provider;
        private Dictionary<Type, bool> protocolSupport = new Dictionary<Type, bool>();
        private readonly IConnection connection;

        public ProtocolManager(IServiceProvider provider, IConnection connection)
        {
            this.provider = provider;
            this.connection = connection;
        }

        public T Get<T>()
             where T : class, IProtocol
        {
            if (this.GetSupport<T>() == ProtocolSupport.Supported)
            {
                return ActivatorUtilities.CreateInstance<T>(this.provider, this, this.connection);
            }
            else
            {
                throw new NotSupportedException($"Protocol '{typeof(T).FullName}' is {this.GetSupport<T>()}");
            }
        }

        public ProtocolSupport GetSupport<T>()
             where T : class, IProtocol
        {
            bool isSupported;

            if (!this.protocolSupport.TryGetValue(typeof(T), out isSupported))
            {
                return ProtocolSupport.Unknown;
            }
            else
            {
                return isSupported
                    ? ProtocolSupport.Supported
                    : ProtocolSupport.NotSupported;
            }
        }

        public void MarkSupport<T>(bool isSupported = true)
            where T : class, IProtocol
        {
            this.protocolSupport[typeof(T)] = isSupported;
        }

        public Task MakeActiveAsync(IProtocol protocol)
        {
            return Task.Run(() => this.Active = protocol);
        }

        public IProtocol Active
        {
            get;
            private set;
        }
    }
}
