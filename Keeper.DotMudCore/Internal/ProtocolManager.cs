using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Keeper.DotMudCore.Internal
{
    internal class ProtocolManager
        : IProtocolManager
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
             where T : class
        {
            if (this.GetSupport<T>() == ProtocolSupport.Supported)
            {
                return ActivatorUtilities.CreateInstance<T>(this.provider, this.connection);
            }
            else
            {
                throw new NotSupportedException($"Protocol '{typeof(T).FullName}' is {this.GetSupport<T>()}");
            }
        }

        public ProtocolSupport GetSupport<T>()
             where T : class
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

        public void MarkSupport<T>(bool isSupported)
            where T : class
        {
            this.protocolSupport[typeof(T)] = isSupported;
        }
    }
}
