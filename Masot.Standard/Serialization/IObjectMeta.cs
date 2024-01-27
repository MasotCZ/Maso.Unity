using System;

namespace Game.Serialization
{

    public interface IObjectMeta
    {
        string Name { get; }
        string Id { get; }
        EMetaTags MetaTag { get; }
        EMetaType MetaTypeName { get; }
    }
}