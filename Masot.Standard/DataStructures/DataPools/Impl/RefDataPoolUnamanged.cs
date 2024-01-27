namespace Masot.Unity.DataStructures.DataPools
{
    public class RefDataPoolUnamanged<_T> : RefDataPoolBase<_T> where _T : class
    {
        public RefDataPoolUnamanged(IRefPoolOptions<_T> options) : base(new RefPoolCollection<_T>(options.DataFactory, options.InitialSize))
        {
        }
    }
}
