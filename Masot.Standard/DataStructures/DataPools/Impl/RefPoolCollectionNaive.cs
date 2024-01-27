using Masot.Framework.DataStructures.Factories;
using System.Collections.Generic;
using System.Linq;

namespace Masot.Unity.DataStructures.DataPools
{
    public class RefPoolCollectionNaive<_T> : IRefPoolCollection<_T> where _T : class
    {
        private readonly ICollection<_T> _used;
        private readonly ICollection<_T> _unused;
        private readonly IDataPoolFactory<_T> _factory;

        public RefPoolCollectionNaive(IDataPoolFactory<_T> factory, int initSize)
        {
            _used = new HashSet<_T>(initSize);
            _unused = new HashSet<_T>(initSize);
            this._factory = factory;
        }

        public int Count => _unused.Count + _used.Count;
        public int Used => _used.Count;

        public IDataPoolFactory<_T> DataFactory => _factory;

        public void Remove(_T item)
        {
            _used.Remove(item);
            _unused.Add(item);
        }

        _T IRefPoolCollection<_T>.Get()
        {
            var ret = _unused.First();
            _unused.Remove(ret);
            _used.Add(ret);

            return ret;
        }
    }
}
