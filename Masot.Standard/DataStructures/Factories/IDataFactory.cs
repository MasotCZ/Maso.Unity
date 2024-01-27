namespace Masot.Framework.DataStructures.Factories
{
    public interface IDataFactory<_T>
    {
        public _T Create();
    }

    public interface IDataPoolFactory<_T> : IDataFactory<_T>
    {
        public void Decommission(_T toDecom);
    }
}
