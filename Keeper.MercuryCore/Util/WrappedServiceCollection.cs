using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Keeper.MercuryCore.Util
{
    public class WrappedServiceCollection<T>
        : IServiceCollection<T>
    {
        private readonly IServiceCollection wrappedCollection;

        public WrappedServiceCollection(IServiceCollection wrappedCollection)
        {
            this.wrappedCollection = wrappedCollection;
        }

        public ServiceDescriptor this[int index]
        {
            get => this.wrappedCollection[index];
            set => this.wrappedCollection[index] = value;
        }

        public int Count => this.wrappedCollection.Count;

        public bool IsReadOnly => this.wrappedCollection.IsReadOnly;

        public void Add(ServiceDescriptor item)
        {
            this.wrappedCollection.Add(item);
        }

        public void Clear()
        {
            this.wrappedCollection.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return this.wrappedCollection.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            this.wrappedCollection.CopyTo(array, arrayIndex);
        }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return this.wrappedCollection.GetEnumerator();
        }

        public int IndexOf(ServiceDescriptor item)
        {
            return this.wrappedCollection.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            this.wrappedCollection.Insert(index, item);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return this.wrappedCollection.Remove(item);
        }

        public void RemoveAt(int index)
        {
            this.wrappedCollection.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.wrappedCollection.GetEnumerator();
        }
    }
}
