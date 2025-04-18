using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Exceptions;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;
using wedding_planer_ad.Models.ViewModels;


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
            try
            {
                return await _context.weddingPlanner
                    .Where(wp => !wp.IsDeleted)
                    .Include(wp => wp.Couple)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all wedding planners.", ex);
            }
        }

        public async Task<WeddingPlanner> GetByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving wedding planner with id {id}.", ex);
            }
        }

        public async Task<WeddingPlanner> CreateAsync(WeddingPlanner planner)
        {
            try
            {
                planner.AssignedDate = DateTime.UtcNow;
                _context.weddingPlanner.Add(planner);
                await _context.SaveChangesAsync();
                return planner;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating wedding planner.", ex);
            }
        }

        public async Task<WeddingPlanner> UpdateAsync(WeddingPlanner planner)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error updating wedding planner.", ex);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var planner = await _context.weddingPlanner.FindAsync(id);

                if (planner == null || planner.IsDeleted)
                    throw new ResourceNotFoundException($"WeddingPlanner with id {id} not found or already deleted.");

                planner.IsDeleted = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting wedding planner.", ex);
            }
        }

        public async Task<IEnumerable<Couple>> GetWeddingsByPlannerUserIdAsync(string plannerUserId)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error retrieving weddings by planner user ID.", ex);
            }
        }

        public async Task<IEnumerable<WeddingChecklist>> GetChecklistsByCoupleIdAsync(int coupleId)
        {
            try
            {
                return await _context.WeddingChecklist
                    .Where(wc => wc.CoupleId == coupleId && !wc.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving checklists by couple ID.", ex);
            }
        }

        public async Task<IEnumerable<CoupleMemberDto>> GetMembersByCoupleIdAsync(int coupleId)
        {
            try
            {
                var members = await _context.CoupleMember
                    .Where(cm => cm.CoupleId == coupleId && !cm.IsDeleted)
                    .ToListAsync();

                var result = new List<CoupleMemberDto>();
                foreach (var member in members)
                {
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
            catch (Exception ex)
            {
                throw new Exception("Error retrieving couple members.", ex);
            }
        }

        public async Task<IEnumerable<Guest>> GetGuestsByCoupleIdAsync(int coupleId)
        {
            try
            {
                return await _context.Guest
                    .Where(g => g.CoupleId == coupleId && !g.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving guests by couple ID.", ex);
            }
        }

        public async Task<IEnumerable<WeddingBudget>> GetBudgetsByCoupleIdAsync(int coupleId)
        {
            try
            {
                return await _context.WeddingBudget
                    .Where(wb => wb.CoupleId == coupleId && !wb.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving budgets by couple ID.", ex);
            }
        }

        public async Task<IEnumerable<WeddingTimeline>> GetTimelineByCoupleIdAsync(int coupleId)
        {
            try
            {
                return await _context.WeddingTimeline
                    .Where(wb => wb.CoupleId == coupleId && !wb.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving timeline by couple ID.", ex);
            }
        }

        public async Task<IEnumerable<Booking>> GetBookingByCoupleId(int coupleId)
        {
            try
            {
                return await _context.Booking
                    .Where(wb => wb.CoupleId == coupleId && !wb.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings by couple ID.", ex);
            }
        }

        public async Task<IEnumerable<BookingVendorDTO>> GetBookingByCoupleIdWithVendor(int coupleId)
        {
            try
            {
                var bookings = await _context.Booking
                    .Where(b => b.CoupleId == coupleId && !b.IsDeleted)
                    .Include(b => b.Vendor)
                    .ToListAsync();

                return bookings.Select(booking => new BookingVendorDTO
                {
                    VendorName = booking.Vendor.Name,
                    VendorDescription = booking.Vendor.Description,
                    VendorPricing = booking.Vendor.Pricing,
                    BookingDate = booking.BookingDate,
                    ServiceDetails = booking.ServiceDetails
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings with vendors.", ex);
            }
        }

        public async Task<WeddingTimeline> GetTimelineByIdAsync(int id)
        {
            try
            {
                return await _context.WeddingTimeline.FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving timeline by ID.", ex);
            }
        }

        public async Task UpdateTimelineAsync(WeddingTimeline timeline)
        {
            try
            {
                _context.WeddingTimeline.Update(timeline);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating timeline.", ex);
            }
        }

        public async Task DeleteTimelineAsync(int id)
        {
            try
            {
                var timeline = await _context.WeddingTimeline.FindAsync(id);
                if (timeline != null)
                {
                    _context.WeddingTimeline.Remove(timeline);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting timeline.", ex);
            }
        }

        public async Task AddTimelineAsync(WeddingTimeline timeline)
        {
            try
            {
                var coupleExists = await _context.Couple.AnyAsync(c => c.Id == timeline.CoupleId);
                if (!coupleExists)
                {
                    throw new ArgumentException("Invalid Couple ID. The couple does not exist.");
                }

                _context.WeddingTimeline.Add(timeline);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding timeline.", ex);
            }
        }

        public async Task<WeddingChecklist> GetChecklistByIdAsync(int id)
        {
            try
            {
                return await _context.WeddingChecklist.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving checklist by ID.", ex);
            }
        }

        public async Task UpdateChecklistAsync(WeddingChecklist checklist)
        {
            try
            {
                _context.WeddingChecklist.Update(checklist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating checklist.", ex);
            }
        }

        public async Task DeleteChecklistAsync(int id)
        {
            try
            {
                var item = await _context.WeddingChecklist.FindAsync(id);
                if (item != null)
                {
                    _context.WeddingChecklist.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting checklist.", ex);
            }
        }

        public async Task AddChecklistTaskAsync(WeddingChecklist checklist)
        {
            try
            {
                checklist.AssignedTo = string.Empty;
                checklist.IsDeleted = false;

                _context.WeddingChecklist.Add(checklist);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding checklist task.", ex);
            }
        }

        public async Task<Dictionary<string, int>> GetWeddingsPerMonthAsync(string plannerUserId)
        {
            try
            {
                var weddings = await _context.weddingPlanner
                    .Include(wp => wp.Couple)
                    .Where(wp => wp.PlannerUserId == plannerUserId && !wp.IsDeleted)
                    .ToListAsync();

                var grouped = weddings
                    .GroupBy(w => w.Couple.WeddingDate.Month)
                    .OrderBy(g => g.Key)
                    .ToDictionary(
                        g => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                        g => g.Count()
                    );

                var allMonths = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames.Take(12).ToList();

                foreach (var month in allMonths)
                {
                    if (!grouped.ContainsKey(month))
                    {
                        grouped.Add(month, 0);
                    }
                }

                return grouped.OrderBy(g => Array.IndexOf(allMonths.ToArray(), g.Key)).ToDictionary(g => g.Key, g => g.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating weddings per month.", ex);
            }
        }

        public async Task<WeddingPlannerDashboardViewModel> GetDashboardDataAsync(string plannerUserId)
        {
            try
            {
                var couples = await _context.Couple
                    .Where(c => c.Planners.PlannerUserId == plannerUserId && !c.IsDeleted)
                    .ToListAsync();

                return new WeddingPlannerDashboardViewModel
                {
                    TotalWeddings = couples.Count,
                    CompletedWeddings = couples.Count(c => c.WeddingDate < DateTime.Now),
                    UpcomingWeddings = couples.Count(c => c.WeddingDate > DateTime.Now),
                    TotalBudget = couples.Sum(c => c.Budget),
                    CompletedWeddingsPerMonth = couples.Where(c => c.WeddingDate < DateTime.Now).GroupBy(c => c.WeddingDate.Month).Select(g => g.Count()).ToList(),
                    UpcomingWeddingsPerMonth = couples.Where(c => c.WeddingDate > DateTime.Now).GroupBy(c => c.WeddingDate.Month).Select(g => g.Count()).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving wedding planner dashboard data.", ex);
            }
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync()
        {
            try
            {
                var couples = await _context.Couple
                    .Where(c => !c.IsDeleted)
                    .ToListAsync();

                return new AdminDashboardViewModel
                {
                    TotalWeddings = couples.Count,
                    CompletedWeddings = couples.Count(c => c.WeddingDate < DateTime.Now),
                    WeddingsPerMonth = couples
                        .GroupBy(c => c.WeddingDate.Month)
                        .OrderBy(g => g.Key)
                        .ToDictionary(
                            g => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                            g => g.Count()
                        )
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving admin dashboard data.", ex);
            }
        }

        public async Task<ApplicationUser> GetCoupleByUserId(int coupleId)
        {
            try
            {
                var couple = await _context.Couple.FirstOrDefaultAsync(c => c.Id == coupleId);

                if (couple == null || string.IsNullOrEmpty(couple.UserId))
                    return null;

                var user = await _userManager.FindByIdAsync(couple.UserId);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving couple by user ID.", ex);
            }
        }
    }
}

