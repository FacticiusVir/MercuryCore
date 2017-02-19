using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Keeper.DotMudCore.Protocols.Internal
{
    internal class ProtocolManager
        : IProtocolManager, IProtocolManagerControl
    {
        private readonly ILogger<ProtocolManager> logger;
        private readonly IServiceProvider provider;
        private readonly IConnection connection;
        private readonly Dictionary<Type, object> protocolSupport = new Dictionary<Type, object>();

        private IDisposable activeProtocolSession;

        public ProtocolManager(ILogger<ProtocolManager> logger, IServiceProvider provider, IConnection connection)
        {
            this.logger = logger;
            this.provider = provider;
            this.connection = connection;
        }

        public T Get<T>()
             where T : class, IProtocol
        {
            if (this.GetSupport<T>() == ProtocolSupport.Supported)
            {
                return (T)this.protocolSupport[typeof(T)];
            }
            else
            {
                throw new NotSupportedException($"Protocol '{typeof(T).FullName}' is {this.GetSupport<T>()}");
            }
        }

        public ProtocolSupport GetSupport<T>()
             where T : class, IProtocol
        {
            object instance;

            if (!this.protocolSupport.TryGetValue(typeof(T), out instance))
            {
                return ProtocolSupport.Unknown;
            }
            else
            {
                return instance != null
                    ? ProtocolSupport.Supported
                    : ProtocolSupport.NotSupported;
            }
        }

        public void MarkSupport<T, V>()
            where T : class, IProtocol
            where V : T
        {
            this.protocolSupport[typeof(T)] = ActivatorUtilities.CreateInstance<V>(this.provider, this, this.connection);

            this.LogSupport<T>();
        }

        public void MarkNotSupported<T>()
            where T : class, IProtocol
        {
            this.protocolSupport[typeof(T)] = null;

            this.LogSupport<T>();
        }

        public Task MakeActiveAsync(IProtocol protocol)
        {
            if (this.Active != protocol)
            {
                if(this.activeProtocolSession != null)
                {
                    this.activeProtocolSession.Dispose();
                }

                this.Active = protocol;

                this.activeProtocolSession = protocol.CreateActiveSession();

                this.logger.LogInformation("Active protocol is {Protocol}.", protocol.GetType().Name);
            }

            return Task.CompletedTask;
        }

        public IProtocol Active
        {
            get;
            private set;
        }

        private void LogSupport<T>() where T : class, IProtocol
        {
            this.logger.LogInformation("Protocol {Protocol} marked as {IsSupported}.", typeof(T).Name, this.GetSupport<T>());
        }
    }
}
