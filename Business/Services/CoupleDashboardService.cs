using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Services
{
    public class CoupleDashboardService : ICoupleDashboardService
    {
        private readonly ApplicationDbContext _context;
        public CoupleDashboardService(ApplicationDbContext context) {
            _context = context;
        }

        public async Task<IEnumerable<WeddingChecklist>> GetChecklistsByCoupleIdAsync(int coupleId)
        {
            return await _context.WeddingChecklist
                .Where(wc => wc.CoupleId == coupleId && !wc.IsDeleted)
                .ToListAsync();
        }

        public async Task<Couple> GetCoupleByUserId(string id)
        {
            return await _context.Couple
                .Include(c => c.Budgets)
                .Include(c => c.Checklists)
                .FirstOrDefaultAsync(c => c.User.Id == id);

        }
    }
}
