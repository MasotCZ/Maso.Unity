using UnityEngine;

namespace Game.Serialization
{

    public abstract class RenderedObjectMetaDataBase : ObjectMetaBase, IRenderedObjectMeta
    {
        public RenderedObjectMetaDataBase() { }
        /// <summary>
        /// TODO path for sprite and prefab
        /// </summary>
        public RenderedObjectMetaDataBase(string iconSprite, string gameObjectPrefab) { }

        [SerializeField]
        private Sprite _iconSprite;
        public Sprite IconSprite => _iconSprite;

        [SerializeField]
        private GameObject _gameObjectPrefab;
        public GameObject GameObjectPrefab => _gameObjectPrefab;
    }
}
