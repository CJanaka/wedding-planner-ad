namespace wedding_planer_ad.Models
{
    public class VendorService
    {
        public int Id { get; set; }
        public Vendor Vendor { get; set; }
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
