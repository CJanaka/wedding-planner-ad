﻿using Microsoft.AspNetCore.Identity;
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

        public async Task<IEnumerable<BookingVendorDTO>> GetBookingByCoupleIdWithVendor(int coupleId)
        {
            // Fetch bookings along with the related vendor details
            var bookings = await _context.Booking
                .Where(b => b.CoupleId == coupleId && !b.IsDeleted)
                .Include(b => b.Vendor)  // Include Vendor details
                .ToListAsync();

            // Map the bookings to BookingVendorDTO with vendor details
            var bookingVendorDTOs = bookings.Select(booking => new BookingVendorDTO
            {
                VendorName = booking.Vendor.Name,
                VendorDescription = booking.Vendor.Description,
                VendorPricing = booking.Vendor.Pricing,
                BookingDate = booking.BookingDate,
                ServiceDetails = booking.ServiceDetails
            }).ToList();

            return bookingVendorDTOs;
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


        public async Task<WeddingChecklist> GetChecklistByIdAsync(int id)
        {
            return await _context.WeddingChecklist.FindAsync(id);
        }

        public async Task UpdateChecklistAsync(WeddingChecklist checklist)
        {
            _context.WeddingChecklist.Update(checklist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteChecklistAsync(int id)
        {
            var item = await _context.WeddingChecklist.FindAsync(id);
            if (item != null)
            {
                _context.WeddingChecklist.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddChecklistTaskAsync(WeddingChecklist checklist)
        {
            checklist.AssignedTo = string.Empty;
            checklist.IsDeleted = false;

            _context.WeddingChecklist.Add(checklist);
            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, int>> GetWeddingsPerMonthAsync(string plannerUserId)
        {
            var weddings = await _context.weddingPlanner
                .Include(wp => wp.Couple)
                .Where(wp => wp.PlannerUserId == plannerUserId && !wp.IsDeleted)
                .ToListAsync();

            var grouped = weddings
                .GroupBy(w => w.Couple.WeddingDate.Month) // Group by month
                .OrderBy(g => g.Key) // Sort by month
                .ToDictionary(
                    g => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key), // Get abbreviated month name
                    g => g.Count() // Get count of weddings for that month
                );

            // To ensure all months are shown, even those without weddings, initialize them with 0 count
            var allMonths = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedMonthNames.Take(12).ToList();

            foreach (var month in allMonths)
            {
                if (!grouped.ContainsKey(month))
                {
                    grouped.Add(month, 0); // Add missing months with 0 count
                }
            }

            // Return the dictionary sorted by month
            return grouped.OrderBy(g => Array.IndexOf(allMonths.ToArray(), g.Key)).ToDictionary(g => g.Key, g => g.Value);
        }



        public async Task<WeddingPlannerDashboardViewModel> GetDashboardDataAsync(string plannerUserId)
        {
            // Get all couples linked to this wedding planner
            var couples = await _context.Couple
                .Where(c => c.Planners.PlannerUserId == plannerUserId && !c.IsDeleted)
                .ToListAsync();

            // Total Weddings
            var totalWeddings = couples.Count;

            // Completed Weddings
            var completedWeddings = couples.Count(c => c.WeddingDate < DateTime.Now);

            // Upcoming Weddings
            var upcomingWeddings = couples.Count(c => c.WeddingDate > DateTime.Now);

            // Total Budget (sum of all couple budgets)
            var totalBudget = couples.Sum(c => c.Budget);

            // Weddings per month (for chart)
            var completedWeddingsPerMonth = couples
                .Where(c => c.WeddingDate < DateTime.Now)
                .GroupBy(c => c.WeddingDate.Month)
                .Select(g => g.Count())
                .ToList();

            var upcomingWeddingsPerMonth = couples
                .Where(c => c.WeddingDate > DateTime.Now)
                .GroupBy(c => c.WeddingDate.Month)
                .Select(g => g.Count())
                .ToList();

            return new WeddingPlannerDashboardViewModel
            {
                TotalWeddings = totalWeddings,
                CompletedWeddings = completedWeddings,
                UpcomingWeddings = upcomingWeddings,
                TotalBudget = totalBudget,
                CompletedWeddingsPerMonth = completedWeddingsPerMonth,
                UpcomingWeddingsPerMonth = upcomingWeddingsPerMonth
            };
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardDataAsync()
        {
            var couples = await _context.Couple
                .Where(c => !c.IsDeleted)
                .ToListAsync();

            int total = couples.Count;
            int completed = couples.Count(c => c.WeddingDate < DateTime.Now);

            var weddingsPerMonth = couples
                .GroupBy(c => c.WeddingDate.Month)
                .OrderBy(g => g.Key)
                .ToDictionary(
                    g => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key),
                    g => g.Count()
                );

            return new AdminDashboardViewModel
            {
                TotalWeddings = total,
                CompletedWeddings = completed,
                WeddingsPerMonth = weddingsPerMonth
            };
        }

        public async Task<ApplicationUser> GetCoupleByUserId(int coupleId)
        {
            var couple = await _context.Couple.FirstOrDefaultAsync(c => c.Id == coupleId);

            if (couple == null || string.IsNullOrEmpty(couple.UserId))
                return null;

            var user = await _userManager.FindByIdAsync(couple.UserId);
            return user;
        }




    }
}
