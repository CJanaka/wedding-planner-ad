namespace wedding_planer_ad.Models.DTO
{
    public class BookingVendorDTO
    {
        public int BookingId { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorDescription { get; set; }
        public string ServiceDetails { get; set; }
        public decimal VendorPricing { get; set; }
        public DateTime BookingDate { get; set; }
    }

}
