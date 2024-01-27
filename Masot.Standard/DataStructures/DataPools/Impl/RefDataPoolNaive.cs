namespace Masot.Unity.DataStructures.DataPools
{
    public class RefDataPoolNaive<_T> : RefDataPoolBase<_T> where _T : class
    {
        public RefDataPoolNaive(IRefPoolOptions<_T> options) : base(new RefPoolCollectionNaive<_T>(options.DataFactory, options.InitialSize) )
        {
        }
    }
}
