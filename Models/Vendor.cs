using Microsoft.AspNetCore.Identity;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        //public string Category { get; set; }
        public string Description { get; set; }
        public decimal Pricing { get; set; }
        public string Location { get; set; }
        public decimal Rating { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<VendorService> Services { get; set; }
        public ICollection<VendorAvailability> Availabilities { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Venue> Venues { get; set; }
        public int CategoryId { get; set; }
        public string ContactEmail { get; set; }
        public string Phone { get; set; }
        public string? WebsiteUrl { get; set; }
        public VendorCat? Category { get; set; }
        public ICollection<VendorPackage>? Packages { get; set; }
        public ICollection<VendorRating>? Ratings { get; set; }
    }
}
