using Microsoft.AspNetCore.Identity;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Models.ViewModels;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<AdminDashboardViewModel> GetDashboardData()
        {
            var couples = _context.Couple.Where(c => !c.IsDeleted).ToList();
            var bookings = _context.Booking.Where(b => !b.IsDeleted).ToList();
            var users = _userManager.Users.ToList();

            var totalPlanners = 0;
            var totalVendors = 0;
            var totalCouples = 0;

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Planner")) totalPlanners++;
                if (await _userManager.IsInRoleAsync(user, "Vendor")) totalVendors++;
                if (await _userManager.IsInRoleAsync(user, "Couple")) totalCouples++;
            }

            var viewModel = new AdminDashboardViewModel
            {
                TotalWeddings = couples.Count,
                CompletedWeddings = couples.Count(c => c.WeddingDate < DateTime.Now),
                WeddingsPerMonth = couples
                    .GroupBy(c => c.WeddingDate.ToString("MMM"))
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.Count()),

                TotalPlanners = totalPlanners,
                TotalVendors = totalVendors,
                TotalCouples = totalCouples,
                TotalBookings = bookings.Count,
                TotalBudget = couples.Sum(c => c.Budget),
                AverageWeddingBudget = couples.Any() ? couples.Average(c => c.Budget) : 0
            };


            return viewModel;
        }

        public async Task<AdminReportViewModel> GetReportData()
        {
            var couples = _context.Couple.Where(c => !c.IsDeleted).ToList();
            var bookings = _context.Booking.Where(b => !b.IsDeleted).ToList();
            var users = _userManager.Users.ToList();

            var totalPlanners = 0;
            var totalVendors = 0;
            var totalCouples = 0;

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Planner")) totalPlanners++;
                if (await _userManager.IsInRoleAsync(user, "Vendor")) totalVendors++;
                if (await _userManager.IsInRoleAsync(user, "Couple")) totalCouples++;
            }

            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            return new AdminReportViewModel
            {
                TotalWeddings = couples.Count,
                CompletedWeddings = couples.Count(c => c.WeddingDate < now),
                TotalPlanners = totalPlanners,
                TotalVendors = totalVendors,
                TotalCouples = totalCouples,
                TotalBookings = bookings.Count,
                TotalBudget = couples.Sum(c => c.Budget),
                AverageWeddingBudget = couples.Any() ? couples.Average(c => c.Budget) : 0,

                WeddingsThisWeek = couples.Count(c => c.WeddingDate >= startOfWeek && c.WeddingDate <= now),
                WeddingsThisMonth = couples.Count(c => c.WeddingDate >= startOfMonth && c.WeddingDate <= now),
                BookingsThisWeek = bookings.Count(b => b.BookingDate >= startOfWeek && b.BookingDate <= now),
                BookingsThisMonth = bookings.Count(b => b.BookingDate >= startOfMonth && b.BookingDate <= now)
            };
        }

        public async Task<AdminReportViewModel> GetWeeklyReportData()
        {
            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var couples = _context.Couple.Where(c => !c.IsDeleted && c.WeddingDate >= startOfWeek && c.WeddingDate <= now).ToList();
            var bookings = _context.Booking.Where(b => !b.IsDeleted && b.BookingDate >= startOfWeek && b.BookingDate <= now).ToList();

            return await GenerateBasicStats(couples, bookings);
        }

        public async Task<AdminReportViewModel> GetMonthlyReportData()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var couples = _context.Couple.Where(c => !c.IsDeleted && c.WeddingDate >= startOfMonth && c.WeddingDate <= now).ToList();
            var bookings = _context.Booking.Where(b => !b.IsDeleted && b.BookingDate >= startOfMonth && b.BookingDate <= now).ToList();

            return await GenerateBasicStats(couples, bookings);
        }

        private async Task<AdminReportViewModel> GenerateBasicStats(List<Couple> couples, List<Booking> bookings)
        {
            var users = _userManager.Users.ToList();
            var totalPlanners = 0;
            var totalVendors = 0;
            var totalCouples = 0;

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Planner")) totalPlanners++;
                if (await _userManager.IsInRoleAsync(user, "Vendor")) totalVendors++;
                if (await _userManager.IsInRoleAsync(user, "Couple")) totalCouples++;
            }

            return new AdminReportViewModel
            {
                TotalWeddings = couples.Count,
                CompletedWeddings = couples.Count(c => c.WeddingDate < DateTime.Now),
                TotalPlanners = totalPlanners,
                TotalVendors = totalVendors,
                TotalCouples = totalCouples,
                TotalBookings = bookings.Count,
                TotalBudget = couples.Sum(c => c.Budget),
                AverageWeddingBudget = couples.Any() ? couples.Average(c => c.Budget) : 0
            };
        }


    }

}
