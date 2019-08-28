using System.Collections.Generic;

namespace Knyaz.Optimus.Environment
{
    interface INamedItemsReadonlyStore<TItem>
    {
        TItem this[int idx] { get; }
        TItem this[string name] { get; }
    }
    
    public class NamedItemsStore<TItem> : INamedItemsReadonlyStore<TItem> where TItem:class
    {
        public NamedItemsStore(IDictionary<string, TItem> items)
        {
            _items = new TItem[items.Count];
            int idx = 0;
            foreach (var pair in items)
            {
                _index.Add(pair.Key, idx);
                _items[idx] = pair.Value;
                idx++;
            }
        }
        
        private readonly TItem[] _items;
        readonly Dictionary<string,int> _index = new Dictionary<string, int>();
        
        public TItem this[string name] =>
            int.TryParse(name, out var number) ? this[number] :
            _index.TryGetValue(name, out var idx) ? _items[idx] : null;

        public TItem this[int idx] => idx < 0 || idx >= _items.Length ? null : _items[idx];

        public int Length => _items.Length;
    }
}