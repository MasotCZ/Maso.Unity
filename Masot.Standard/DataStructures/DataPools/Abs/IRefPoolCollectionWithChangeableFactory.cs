using Masot.Framework.DataStructures.Factories;

namespace Masot.Unity.DataStructures.DataPools
{
    public interface IRefPoolCollectionWithChangeableFactory<_T> : IRefPoolCollection<_T> where _T : class
    {
        public new IDataFactory<_T> DataFactory { get; set; }
    }
}
