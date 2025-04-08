using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<WeddingPlanner> weddingPlanner { get; set; }

        public DbSet<CoupleMember> CoupleMember { get; set; }

        public DbSet<WeddingBudget> WeddingBudget { get; set; }

        public DbSet<WeddingChecklist> WeddingChecklist { get; set; }

        public DbSet<Guest> Guest { get; set; }

        public DbSet<Couple> Couple { get; set; }

        public DbSet<WeddingTimeline> WeddingTimeline { get; set; }

        public DbSet<Booking> Booking { get; set; }
    }
}
