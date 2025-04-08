namespace wedding_planer_ad.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string ReportType { get; set; }
        public DateTime DateRangeStart { get; set; }
        public DateTime DateRangeEnd { get; set; }
        public string ReportData { get; set; }
        public DateTime GeneratedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
