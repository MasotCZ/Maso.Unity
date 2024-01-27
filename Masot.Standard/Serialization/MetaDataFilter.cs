namespace Game.Serialization
{
    public class MetaDataFilter : IMetaDataFilter
    {
        public bool FilterByTag { get; set; } = true;
        public IMetaTagFilters TagFilter { get; set; } = new MetaTagFilters();
    }
}