using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Exceptions;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;

namespace wedding_planer_ad.Business.Services
{
    public class WeddingPlannerService : IWeddingPlannerService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public WeddingPlannerService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<WeddingPlanner>> GetAllAsync()
        {
            return await _context.weddingPlanner
                .Where(wp => !wp.IsDeleted)
                .Include(wp => wp.Couple)
                .ToListAsync();
        }

        public async Task<WeddingPlanner> GetByIdAsync(int id)
        {
            var planner = await _context.weddingPlanner
                .Include(wp => wp.Couple)
                .FirstOrDefaultAsync(wp => wp.Id == id && !wp.IsDeleted);

            if (planner == null)
            {
                throw new ResourceNotFoundException($"WeddingPlanner with id {id} not found.");
            }

            return planner;
        }


        public async Task<WeddingPlanner> CreateAsync(WeddingPlanner planner)
        {
            planner.AssignedDate = DateTime.UtcNow;
            _context.weddingPlanner.Add(planner);
            await _context.SaveChangesAsync();
            return planner;
        }

        public async Task<WeddingPlanner> UpdateAsync(WeddingPlanner planner)
        {
            var existing = await _context.weddingPlanner.FindAsync(planner.Id);

            if (existing == null || existing.IsDeleted)
                throw new ResourceNotFoundException($"WeddingPlanner with id {planner.Id} not found.");

            if (!string.IsNullOrEmpty(planner.PlannerUserId))
                existing.PlannerUserId = planner.PlannerUserId;

            if (planner.CoupleId != 0)
                existing.CoupleId = planner.CoupleId;

            if (planner.AssignedDate != default)
                existing.AssignedDate = planner.AssignedDate;


            await _context.SaveChangesAsync();
            return existing;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var planner = await _context.weddingPlanner.FindAsync(id);

            if (planner == null || planner.IsDeleted)
                throw new ResourceNotFoundException($"WeddingPlanner with id {id} not found or already deleted.");

            planner.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Couple>> GetWeddingsByPlannerUserIdAsync(string plannerUserId)
        {
            return await _context.weddingPlanner
                .Where(wp => wp.PlannerUserId == plannerUserId && !wp.IsDeleted)
                .Include(wp => wp.Couple)
                    .ThenInclude(c => c.Guests)
                .Include(wp => wp.Couple.Checklists)
                .Include(wp => wp.Couple.Bookings)
                .Select(wp => wp.Couple)
                .ToListAsync();
        }



        public async Task<IEnumerable<WeddingChecklist>> GetChecklistsByCoupleIdAsync(int coupleId)
        {
            return await _context.WeddingChecklist
                .Where(wc => wc.CoupleId == coupleId && !wc.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<CoupleMemberDto>> GetMembersByCoupleIdAsync(int coupleId)
        {
            var members = await _context.CoupleMember
                .Where(cm => cm.CoupleId == coupleId && !cm.IsDeleted)
                .ToListAsync();

            var result = new List<CoupleMemberDto>();
            Console.Write(result);

            foreach (var member in members)
            {
                Console.WriteLine(member);

                var user = await _userManager.FindByIdAsync(member.UserId);
                if (user != null)
                {
                    result.Add(new CoupleMemberDto
                    {
                        Id = member.Id,
                        UserId = member.UserId,
                        UserName = user.UserName ?? string.Empty,
                        Email = user.Email ?? string.Empty
                    });
                }
            }

            return result;
        }

        public async Task<IEnumerable<Guest>> GetGuestsByCoupleIdAsync(int coupleId)
        {
            return await _context.Guest
                .Where(g => g.CoupleId == coupleId && !g.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<WeddingBudget>> GetBudgetsByCoupleIdAsync(int coupleId)
        {
            return await _context.WeddingBudget
                .Where(wb => wb.CoupleId == coupleId && !wb.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<WeddingTimeline>> GetTimelineByCoupleIdAsync(int coupleId)
        {
            return await _context.WeddingTimeline
                .Where(wb => wb.CoupleId == coupleId && !wb.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingByCoupleId(int coupleId)
        {
            return await _context.Booking
                .Where(wb => wb.CoupleId == coupleId && !wb.IsDeleted)
                .ToListAsync();
        }

        public async Task<WeddingTimeline> GetTimelineByIdAsync(int id)
        {
            return await _context.WeddingTimeline.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateTimelineAsync(WeddingTimeline timeline)
        {
            _context.WeddingTimeline.Update(timeline);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTimelineAsync(int id)
        {
            var timeline = await _context.WeddingTimeline.FindAsync(id);
            if (timeline != null)
            {
                _context.WeddingTimeline.Remove(timeline);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddTimelineAsync(WeddingTimeline timeline)
        {
            var coupleExists = await _context.Couple.AnyAsync(c => c.Id == timeline.CoupleId);
            Console.WriteLine($"CoupleId {timeline.CoupleId} exists: {coupleExists}");

            if (!coupleExists)
            {
                Console.WriteLine($"Invalid Couple ID: {timeline.CoupleId}");
                throw new ArgumentException("Invalid Couple ID. The couple does not exist.");
            }

            _context.WeddingTimeline.Add(timeline);
            await _context.SaveChangesAsync();
            Console.WriteLine("Timeline added successfully.");
        }






    }
}
