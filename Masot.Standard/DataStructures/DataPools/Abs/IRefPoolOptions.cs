using Masot.Framework.DataStructures.Factories;

namespace Masot.Unity.DataStructures.DataPools
{
    public interface IRefPoolOptions<_T>
    {
        public IDataPoolFactory<_T> DataFactory { get; }
        public int InitialSize { get; }
    }
}
