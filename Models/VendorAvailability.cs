namespace wedding_planer_ad.Models
{
    public class VendorAvailability
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public DateTime AvailableDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
