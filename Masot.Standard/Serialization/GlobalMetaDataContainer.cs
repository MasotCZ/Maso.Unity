//using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

namespace Game.Serialization
{

    public class GlobalMetaDataContainer : IGlobalMetaContainer
    {
        //cant rly interface it
        private readonly IDictionary<string, IObjectMeta> _data;
        private static GlobalMetaDataContainer _instance;

        public static GlobalMetaDataContainer Instance
        {
            get
            {
                if (_instance is null)
                {
                    _instance = new GlobalMetaDataContainer();
                }

                return _instance;
            }
        }

        public IDictionary<Type, IEnumerable<IObjectMeta>> AllData => throw new NotImplementedException();

        public GlobalMetaDataContainer()
        {
            _data = new Dictionary<string, IObjectMeta>();

            //put inits here for the json files to load from and define the meta data structure
            //Init<VisualProgrammingPropertyMeta>(Settings.Values.JSON_VISUAL_PROGRAMMING.Value);
        }

        public void Init<_T>(TextAsset jsonFile) where _T : class, IObjectMeta
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

        //call this at start of the scene
        public void CreateDictionary()
        {
            foreach (var item in preLoadMetaData)
            {
                RegisterMetaData(item);
            }

            preLoadMetaData.Clear();
        }

        public _T MetaData<_T>(string id) where _T : class, IObjectMeta
        {
            return _data[id] as _T;
        }

        public IEnumerable<_T> MetaList<_T>(IMetaDataFilter filter = null) where _T : class, IObjectMeta
        {
            //if (!index.ContainsKey(typeof(_T)))
            //{
            //    return new _T[0];
            //}

            var type = typeof(_T);

            var ret = _data
                .Values
                .Where(d => type.IsAssignableFrom(d.GetType()))
                .Select(d => d);

            if (ret is null || ret.Count() == 0)
            {
                return new _T[0];
            }

            if (filter == null)
            {
                return ret.Select(d => d as _T);
            }

            foreach (var tagfilter in filter.TagFilter)
            {
                if (ret is null || ret.Count() == 0)
                {
                    break;
                }

                ret = ret.Where(tagfilter.ConstraintFunction);
                //ret = ret.Select(d => new Tuple<_T, IMetaTagFilter>(d, tagfilter)).Where(tagfilter.ConstraintFunction);
            }

            if (ret is null)
            {
                ret = new _T[0];
            }

            var toRet = ret.Select(d => d as _T).ToArray();
            return toRet;
        }

        //Type param not needed but maybe usefull in the future
        public void RegisterMetaData<_T>(_T metadata) where _T : class, IObjectMeta
        {
            _data.Add(metadata.Id, metadata);
        }

        private readonly ICollection<ObjectMetaBase> preLoadMetaData = new List<ObjectMetaBase>();
        public void RegisterPreLoadMetaDataObject(ObjectMetaBase metaData)
        {
            preLoadMetaData.Add(metaData);
        }

        public bool RemoveMetaData(string id)
        {
            return _data.Remove(id);
        }
    }
}
