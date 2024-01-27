using System;

namespace Game.Serialization
{
    public interface IMetaTagFilter
    {
        EFilterType FilterType { get; }
        EMetaTags MetaTag { get; }
        Func<IObjectMeta, bool> ConstraintFunction { get; }
    }
}