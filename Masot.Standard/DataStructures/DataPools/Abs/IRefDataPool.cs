namespace Masot.Unity.DataStructures.DataPools
{
    public interface IRefDataPool<_T> where _T : class
    {
        public _T GetOrCreate();
        public void Decommission(_T toDecom);
    }
}
