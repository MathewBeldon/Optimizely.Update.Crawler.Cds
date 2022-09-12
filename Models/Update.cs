namespace Optimizely.Update.Crawler.Cds.Models
{
    internal class Update
    {
        public int UpdateNumber { get; set; }
        public string UpdateUrl { get; set; }
        public DateTime UpdateDate { get; set; }
        public int CommerceUpdateCount { get; set; }
        public List<RowData> RowData { get; set; } = new List<RowData>();
    }
}
