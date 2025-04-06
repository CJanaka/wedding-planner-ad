namespace wedding_planer_ad.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
    }
}
