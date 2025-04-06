namespace wedding_planer_ad.Models
{
    public class WeddingTimeline
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public string EventName { get; set; }
        public DateTime EventTime { get; set; }
        public string EventDescription { get; set; }
        public bool IsDeleted { get; set; }
    }
}
