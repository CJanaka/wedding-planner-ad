namespace wedding_planer_ad.Models.ViewModels
{
    public class VendorDashboardViewModel
    {
        public Vendor CurrentVendor { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<VendorService> Services { get; set; }
        public int PendingBookingsCount { get; set; }
        public int ConfirmedBookingsCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
