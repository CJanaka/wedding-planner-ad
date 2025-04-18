using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
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
                booking.Status = "Pending";
                booking.PaymentStatus = "Not Paid";
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
                .Where(b => b.CoupleId == couple.Id && !b.IsDeleted)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }
    }
}
