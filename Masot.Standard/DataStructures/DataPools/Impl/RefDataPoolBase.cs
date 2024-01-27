namespace Masot.Unity.DataStructures.DataPools
{
    public abstract class RefDataPoolBase<_T> : IRefDataPool<_T> where _T : class
    {
        private readonly IRefPoolCollection<_T> _pool;

        protected RefDataPoolBase(IRefPoolCollection<_T> pool)
        {
            _pool = pool;
        }

        public void Decommission(_T toDecom)
        {
            _pool.Remove(toDecom);
            _pool.DataFactory.Decommission(toDecom);
        }

        public _T GetOrCreate()
            => _pool.Get();
    }
}
