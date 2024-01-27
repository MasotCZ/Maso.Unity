using Masot.Framework.DataStructures.Factories;

namespace Masot.Unity.DataStructures.DataPools
{
    public interface IRefPoolCollection<_T> where _T : class
    {
        public int Count { get; }
        public int Used { get; }

        public IDataPoolFactory<_T> DataFactory { get; }

        internal _T Get();
        public void Remove(_T item);
    }
}
