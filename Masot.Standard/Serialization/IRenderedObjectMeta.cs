using UnityEngine;

namespace Game.Serialization
{
    public interface IRenderedObjectMeta : IObjectMeta
    {
        Sprite IconSprite { get; }
        GameObject GameObjectPrefab { get; }
    }
}