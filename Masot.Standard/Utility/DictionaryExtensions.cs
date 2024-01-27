using System.Collections.Generic;

namespace Masot.Standard.Utility
{
    public static class DictionaryExtensions
    {
        public static void AddToSubCollection<_K, _V>(this Dictionary<_K, HashSet<_V>> dict, _K key, _V value)
        {
            if (!dict.ContainsKey(key))
            {
                dict.Add(key, new HashSet<_V>());
            }

            dict[key].Add(value);
        }

        public static void RemoveFromSubCollection<_K, _V>(this Dictionary<_K, HashSet<_V>> dict, _K key, _V value)
        {
            if (dict.ContainsKey(key))
            {
                var ll = dict[key];
                if (!ll.Contains(value))
                {
                    return;
                }
                ll.Remove(value);
                if (ll.Count == 0)
                {
                    dict.Remove(key);
                }
            }
        }
    }
}
