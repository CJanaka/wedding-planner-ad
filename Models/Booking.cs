namespace wedding_planer_ad.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int CoupleId { get; set; }
        public int VendorId { get; set; }
        public int ServiceId { get; set; }
        public DateTime BookingDate { get; set; }
        public string ServiceDetails { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
