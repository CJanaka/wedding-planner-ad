using Microsoft.AspNetCore.Identity;

namespace wedding_planer_ad.Models
{
    public class VendorRating
    {
        public int Id { get; set; }

        public int VendorId { get; set; }
        public string UserId { get; set; }          // FK to ApplicationUser
        public int Rating { get; set; }             // 1 to 5 stars
        public string? Review { get; set; }
        // Navigation
        public Vendor Vendor { get; set; }
        public ApplicationUser User { get; set; }
    }

}