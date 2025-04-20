using System.Diagnostics;
using System.Numerics;
using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Exceptions;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.ViewModels;

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
                .Include(c => c.Timelines)
                .FirstOrDefaultAsync(c => c.User.Id == id);

        }

        public async Task<VendorBookingViewModel> GetVendorsAsync(string searchString, int? categoryId, string location, string sortOrder)
        {
            // Start with all vendors that aren't deleted
            var vendors = _context.Vendor
                .Include(v => v.Category)
                .Include(v => v.Ratings)
                .Include(v => v.Reviews)
                .Include(v => v.Services)
                .Where(v => !v.IsDeleted)
                .AsQueryable();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(searchString))
            {
                vendors = vendors.Where(v => v.Name.Contains(searchString) ||
                                            v.Description.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                vendors = vendors.Where(v => v.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(location))
            {
                vendors = vendors.Where(v => v.Location.Contains(location));
            }

            // Apply sorting
            switch (sortOrder)
            {
                case "rating_desc":
                    vendors = vendors.OrderByDescending(v => v.Rating);
                    break;
                case "price_asc":
                    vendors = vendors.OrderBy(v => v.Pricing);
                    break;
                case "price_desc":
                    vendors = vendors.OrderByDescending(v => v.Pricing);
                    break;
                case "name_asc":
                    vendors = vendors.OrderBy(v => v.Name);
                    break;
                default: // Default to highest rated
                    vendors = vendors.OrderByDescending(v => v.Rating);
                    break;
            }

            // Get all categories for the filter dropdown
            var categories = await _context.VendorCat.ToListAsync();

            return new VendorBookingViewModel
            {
                Vendors = await vendors.ToListAsync(),
                Categories = categories,
                SearchString = searchString,
                CategoryId = categoryId,
                Location = location,
                SortOrder = sortOrder
            };
        }

        public async Task<VendorDetailsViewModel> GetVendorDetailsAsync(int id, string userId)
        {
            var vendor = await _context.Vendor
                .Include(v => v.Category)
                .Include(v => v.Services)
                .Include(v => v.Packages)
                .Include(v => v.Reviews)
                .Include(v => v.Availabilities)
                .Include(v => v.Ratings)
                .FirstOrDefaultAsync(v => v.Id == id && !v.IsDeleted);

            if (vendor == null)
            {
                return null;
            }

            // Get the couple ID for the current user
            var couple = await _context.Couple.FirstOrDefaultAsync(c => c.UserId == userId);

            return new VendorDetailsViewModel
            {
                Vendor = vendor,
                Booking = new Booking
                {
                    VendorId = vendor.Id,
                    CoupleId = couple?.Id ?? 0,
                    BookingDate = DateTime.Now
                }
            };
        }

        public async Task<bool> BookVendorAsync(Booking booking)
        {
            if (booking == null)
            {
                return false;
            }

            try
            {
                booking.BookingDate = DateTime.Now;
                booking.IsDeleted = false;

                _context.Booking.Add(booking);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Booking>> GetCoupleBookingsAsync(string userId)
        {
            var couple = await _context.Couple.FirstOrDefaultAsync(c => c.UserId == userId);

            if (couple == null)
            {
                return new List<Booking>();
            }

            return await _context.Booking
                .Include(b => b.Vendor)
                .Include(b => b.Payments)
                .Include(b => b.Vendor.Services)
                .Where(b => b.CoupleId == couple.Id && !b.IsDeleted)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync() ?? new List<Booking>(); ;
        }

        public async Task<Couple> CreateAsync(Couple couple)
        {
            couple.CreatedAt = DateTime.Now;
            couple.UpdatedAt = DateTime.Now;
            _context.Couple.Add(couple);
            await _context.SaveChangesAsync();
            return couple;
        }

        public async Task<Couple> UpdateAsync(Couple couple)
        {
            var existing = await _context.Couple.FindAsync(couple.Id);

            if (existing == null || existing.IsDeleted)
                throw new ResourceNotFoundException($"couple with id {couple.Id} not found.");

            existing.UpdatedAt = DateTime.UtcNow;
            existing.WeddingDate = couple.WeddingDate;
            existing.Budget = couple.Budget;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<WeddingBudget> UpdateBudjetAsync(WeddingBudget WeddingBudget)
        {
            var existing = await _context.WeddingBudget.FindAsync(WeddingBudget.Id);
            
            if (existing.AllocatedAmount != WeddingBudget.AllocatedAmount) {
            
                existing.AllocatedAmount = WeddingBudget.AllocatedAmount;
            }
            existing.SpentAmount = WeddingBudget.SpentAmount;

            await _context.SaveChangesAsync();
            return existing;

        }

        public async Task<Couple> GetCoupleById(int id)
        {
            return await _context.Couple
                .Include(c => c.Budgets)
                .Include(c => c.Checklists)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> UpdateBookingAsync(Booking booking)
        {
            var existing = await _context.Booking.FindAsync(booking.Id);

            if (existing == null)
            {
                Debug.WriteLine("booking not found! id- "+ booking.Id);
                return false;
            }

            try
            {
                if(booking.Reviews != null)
                    existing.Reviews = booking.Reviews;

                if(booking.Status != null)
                    existing.Status = booking.Status;

                if(existing.PaymentStatus != null)
                    existing.PaymentStatus = booking.PaymentStatus;

                if (existing.BookingDate != null)
                    existing.BookingDate = booking.BookingDate;

                existing.IsDeleted = booking.IsDeleted;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<WeddingBudget> GetBudgetsByIdAsync(int id)
        {
            return await _context.WeddingBudget
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<WeddingBudget> CreateBudjetAsync(WeddingBudget weddingBudget)
        {

            _context.WeddingBudget.Add(weddingBudget);
            await _context.SaveChangesAsync();
            return weddingBudget;
        }
    }
}
