namespace wedding_planer_ad.Models
{
    public class SystemUsageLog
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string Metadata { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
