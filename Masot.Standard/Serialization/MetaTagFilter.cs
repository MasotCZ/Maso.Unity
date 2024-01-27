using System;

namespace Game.Serialization
{
    public class MetaTagFilter : IMetaTagFilter
    {
        public MetaTagFilter(EFilterType filterType, EMetaTags metaTag)
        {
            FilterType = filterType;
            MetaTag = metaTag;

            switch (FilterType)
            {
                case EFilterType.HasOnly:
                    ConstraintFunction = (d) =>
                    {
                        return (d.MetaTag & MetaTag) == d.MetaTag;
                    };
                    break;
                case EFilterType.Contains:
                    ConstraintFunction = (d) =>
                    {
                        return (d.MetaTag & MetaTag) > 0;
                    };
                    break;
                case EFilterType.DoesNotContain:
                    ConstraintFunction = (d) =>
                    {
                        return (d.MetaTag & MetaTag) == 0;
                    };
                    break;
                default:
                    ConstraintFunction = (d) =>
                    {
                        return false;
                    };
                    break;
            }
        }

        public EFilterType FilterType { get; }
        public EMetaTags MetaTag { get; }
        public Func<IObjectMeta, bool> ConstraintFunction { get; }
    }
}