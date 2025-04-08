namespace wedding_planer_ad.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string Address { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int Capacity { get; set; }
        public string IndoorOutdoor { get; set; }
        public string Amenities { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
