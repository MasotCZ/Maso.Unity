namespace Game.Serialization
{
    public interface IMetaDataFilter
    {
        IMetaTagFilters TagFilter { get; }
        bool FilterByTag { get; }
    }
}