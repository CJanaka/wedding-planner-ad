using Microsoft.AspNetCore.Identity;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Role { get; set; } // "Couple", "Planner", "Vendor", "Admin"
        public string ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public ICollection<CoupleMember> CoupleMemberships { get; set; }
        public ICollection<WeddingPlanner> PlannerAssignments { get; set; }
        public ICollection<Vendor> VendorProfiles { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceivedMessages { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }
        public ICollection<SystemUsageLog> UsageLogs { get; set; }
    }
}