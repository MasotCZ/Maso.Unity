using System.Collections.Generic;

namespace Game.Serialization
{
    interface IGlobalMetaIndex<_T>
    {
        _T MetaData(string id);
        IEnumerable<_T> MetaList(IMetaDataFilter filter = null);
        void RegisterMetaData(_T metadata);
        void RemoveMetaData(string id);
        IEnumerable<_T> TotalData { get; }
    }
}