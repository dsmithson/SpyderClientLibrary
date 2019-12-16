using System.Collections.Generic;
using System.Linq;

namespace Spyder.Client.Collections
{
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> items;

        public TKey Key
        {
            get;
            private set;
        }

        public Grouping(TKey key, IEnumerable<TElement> items)
        {
            this.Key = key;
            this.items = items;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}
