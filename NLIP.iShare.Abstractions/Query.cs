namespace NLIP.iShare.Abstractions
{
    public class Query
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string SortBy { get; set; }
        public string Filter { get; set; }
        public SortOrder SortOrder { get; set; }
    }
}
