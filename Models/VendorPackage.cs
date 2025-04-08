using Microsoft.AspNetCore.Identity;

namespace wedding_planer_ad.Models
{
    public class VendorPackage
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public string Title { get; set; } // e.g., "Gold Wedding Package"
        public string Description { get; set; }
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; } // Store file path or CDN URL

        public Vendor? Vendor { get; set; }
    }

}