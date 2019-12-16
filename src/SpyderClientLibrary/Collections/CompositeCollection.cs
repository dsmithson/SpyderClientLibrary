using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Spyder.Client.Collections
{
    public class CompositeCollection : ICollection, INotifyCollectionChanged
    {
        private readonly object syncRoot = new object();
        private readonly ObservableCollection<IList> collections = new ObservableCollection<IList>();
        private readonly List<INotifyCollectionChanged> registeredNotifyingCollections = new List<INotifyCollectionChanged>();
        public ObservableCollection<IList> Collections
        {
            get { return collections; }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, e);
        }

        public CompositeCollection()
        {
            collections.CollectionChanged += collections_CollectionChanged;
        }

        public void Add(IList collection)
        {
            Remove(collection);
            collections.Add(collection);
        }

        public void RemoveAt(int index)
        {
            collections.RemoveAt(index);
        }

        public void Remove(IList collection)
        {
            if (collections.Contains(collection))
                collections.Remove(collection);
        }

        public void Clear()
        {
            collections.Clear();
        }

        void notifying_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void collections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //De-register all items
                while (registeredNotifyingCollections.Count > 0)
                {
                    registeredNotifyingCollections[0].CollectionChanged -= observable_CollectionChanged;
                    registeredNotifyingCollections.RemoveAt(0);
                }
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (IList collection in e.OldItems)
                {
                    var observable = collection as INotifyCollectionChanged;
                    if (observable != null)
                    {
                        observable.CollectionChanged -= observable_CollectionChanged;
                        if (registeredNotifyingCollections.Contains(observable))
                            registeredNotifyingCollections.Remove(observable);
                    }
                }
            }

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (IList collection in e.NewItems)
                {
                    var observable = collection as INotifyCollectionChanged;
                    if (observable != null)
                    {
                        observable.CollectionChanged += observable_CollectionChanged;
                        registeredNotifyingCollections.Add(observable);
                    }
                }
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        void observable_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void CopyTo(Array array, int index)
        {
            foreach (object item in this)
            {
                array.SetValue(item, index++);
            }
        }

        public int Count
        {
            get { return collections.Sum(collection => collection.Count); }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        public IEnumerator GetEnumerator()
        {
            return new CompositeCollectionEnumerator(collections);
        }

        protected class CompositeCollectionEnumerator : IEnumerator
        {
            private readonly ObservableCollection<IList> collections;
            private IEnumerator currentEnumerator;
            private int currentCollectionIndex;

            public CompositeCollectionEnumerator(ObservableCollection<IList> collections)
            {
                this.collections = collections;
                Reset();
            }

            public object Current
            {
                get { return (currentEnumerator == null ? null : currentEnumerator.Current); }
            }

            public bool MoveNext()
            {
                if (currentEnumerator == null)
                    return false;

                if (currentEnumerator.MoveNext())
                    return true;

                //If we returned false above, lets move to the next collection
                while (++currentCollectionIndex < collections.Count)
                {
                    //Lets move to the next collection, and try to move it to the first position
                    currentEnumerator = collections[currentCollectionIndex].GetEnumerator();
                    if (currentEnumerator.MoveNext())
                        return true;
                }

                //No more collections to iterate
                return false;
            }

            public void Reset()
            {
                currentEnumerator = (collections.Count == 0 ? null : collections[0].GetEnumerator());
            }
        }
    }
}
