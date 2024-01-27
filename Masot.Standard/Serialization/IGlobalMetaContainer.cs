using System.Collections.Generic;
using System;
using UnityEngine;

namespace Game.Serialization
{
    public interface IGlobalMetaContainer
    {
        void Init<_T>(TextAsset jsonFile) where _T : class, IObjectMeta;
        _T MetaData<_T>(string id) where _T : class, IObjectMeta;
        IEnumerable<_T> MetaList<_T>(IMetaDataFilter filter = null) where _T : class, IObjectMeta;
        void RegisterMetaData<_T>(_T metadata) where _T : class, IObjectMeta;
        bool RemoveMetaData(string id);
        IDictionary<Type, IEnumerable<IObjectMeta>> AllData { get; }
        void RegisterPreLoadMetaDataObject(ObjectMetaBase metaData);
    }
}