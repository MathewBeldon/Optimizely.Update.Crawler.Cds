namespace Optimizely.Update.Crawler.Cds.Models
{
    internal class RowData
    {
        public string Id { get; set; }
        public string Area { get; set; }
        public string DescriptionTitle { get; set; }
        public IEnumerable<string> Description { get; set; }
        public string Type { get; set; }
        public DateTime Released { get; set; }
    }
}
