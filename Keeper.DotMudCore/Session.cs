using System;
using System.Collections.Generic;

namespace Keeper.DotMudCore
{
    internal class Session
        : ISession
    {
        private Dictionary<Type, object> sessionState = new Dictionary<Type, object>();

        public IConnection Connection
        {
            get;
            private set;
        }

        public Session(IConnection connection)
        {
            this.Connection = connection;
        }

        public T GetState<T>()
        {
            return (T)this.sessionState[typeof(T)];
        }

        public bool TryGetState<T>(out T value)
        {
            value = default(T);

            object storedValue;

            if (!this.sessionState.TryGetValue(typeof(T), out storedValue))
            {
                return false;
            }
            else
            {
                value = (T)storedValue;

                return true;
            }
        }

        public void RemoveState<T>()
        {
            this.sessionState.Remove(typeof(T));
        }

        public void SetState<T>(T value)
        {
            this.sessionState[typeof(T)] = value;
        }
    }
}
