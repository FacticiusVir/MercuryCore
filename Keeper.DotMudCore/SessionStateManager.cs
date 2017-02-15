using System;
using System.Collections.Generic;
using System.Text;

namespace Keeper.DotMudCore
{
    public class SessionStateManager
        : ISessionStateManager
    {
        private Dictionary<Type, object> sessionState = new Dictionary<Type, object>();

        public T Get<T>()
        {
            return (T)this.sessionState[typeof(T)];
        }

        public bool TryGet<T>(out T value)
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

        public void Remove<T>()
        {
            this.sessionState.Remove(typeof(T));
        }

        public void Set<T>(T value)
        {
            this.sessionState[typeof(T)] = value;
        }
    }
}
