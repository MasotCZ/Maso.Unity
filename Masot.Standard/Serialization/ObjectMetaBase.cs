using Masot.Standard.Editor;
using System;
using UnityEngine;

namespace Game.Serialization
{
    public abstract class ObjectMetaBase : ScriptableObject, IObjectMeta
    {
        public ObjectMetaBase() { }
        public ObjectMetaBase(string name, string description)
        {
            _name = name;
            _description = description;
        }

        [SerializeField]
        private string _name = "name";
        public string Name => _name;

        [SerializeField]
        private string _description = "description";
        public string Description => _description;

        [SerializeField]
        [ReadOnly]
        private string _id = Guid.NewGuid().ToString();
        public string Id => _id;

        [SerializeField]
        private EMetaTags _metaTag;
        public EMetaTags MetaTag => _metaTag;

        [SerializeField]
        private EMetaType _metaTypeName;
        public EMetaType MetaTypeName => _metaTypeName;

        protected virtual void OnEnable()
            => RegisterMeta();

        private void RegisterMeta()
            => GlobalMetaDataContainer.Instance.RegisterPreLoadMetaDataObject(this);
    }
}