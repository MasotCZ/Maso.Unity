using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

namespace Game.Serialization
{
    public abstract class GlobalMetaIndex<_T> : IGlobalMetaIndex<_T> where _T : ObjectMetaBase
    {
        private readonly Dictionary<string, _T> index = new Dictionary<string, _T>();

        protected GlobalMetaIndex(TextAsset jsonFile)
        {
            Init(index, jsonFile);
        }

        protected virtual void Init(Dictionary<string, _T> index, TextAsset jsonFile)
        {
            if (jsonFile is null)
            {
                return;
            }

            var data = JsonUtility.FromJson(new StringReader(jsonFile.text).ReadToEnd(), typeof(MetaHolder<_T>)) as MetaHolder<_T>;
            foreach (var item in data.Data)
            {
                RegisterMetaData(item);
            }
        }

        public IEnumerable<_T> TotalData => index.Values;

        public _T MetaData(string id)
        {
            return index[id];
        }

        public IEnumerable<_T> MetaList(IMetaDataFilter filter = null)
        {
            if (filter == null)
            {
                return index.Values;
            }

            IEnumerable<_T> ret = new List<_T>(index.Values);

            foreach (var tagfilter in filter.TagFilter)
            {
                ret = ret.Where(tagfilter.ConstraintFunction) as IEnumerable<_T>;
                //ret = ret.Select(d => new Tuple<_T, IMetaTagFilter>(d, tagfilter)).Where(tagfilter.ConstraintFunction);
            }

            return ret;
        }

        public virtual void RegisterMetaData(_T metadata)
        {
            index.Add(metadata.Id, metadata);
        }
        public virtual void RemoveMetaData(string id)
        {
            index.Remove(id);
        }
    }
}