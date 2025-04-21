using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Data;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.ViewModels;

namespace wedding_planer_ad.Business.Services
{
    public class VendorServices : IVendorServices
    {
        private readonly ApplicationDbContext _context;

        public VendorServices(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public async Task<Vendor> GetById(int id)
        {
            return await _context.Vendor.FirstOrDefaultAsync(t => t.Id == id);

        }

        public async Task<Vendor> GetVendorByUserIdAsync(string userId)
        {
            return await _context.Vendor
                .Include(v => v.Category)
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public async Task<VendorDashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var vendor = await GetVendorByUserIdAsync(userId);

            if (vendor == null)
            {
                return null;
            }

            var bookings = await _context.Booking
                .Where(b => b.VendorId == vendor.Id && !b.IsDeleted)
                .Include(b => b.Payments)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var services = await _context.VendorService
                .Where(s => s.Vendor.Id == vendor.Id && !s.IsDeleted)
                .ToListAsync();

            var pendingBookingsCount = bookings.Count(b => b.Status == "Pending");
            var confirmedBookingsCount = bookings.Count(b => b.Status == "Confirmed");
            var totalRevenue = bookings
                .Where(b => b.Status == "Confirmed" && b.PaymentStatus == "Paid")
                .Sum(b => b.TotalAmount);

            return new VendorDashboardViewModel
            {
                CurrentVendor = vendor,
                Bookings = bookings,
                Services = services,
                PendingBookingsCount = pendingBookingsCount,
                ConfirmedBookingsCount = confirmedBookingsCount,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<bool> ConfirmBookingAsync(int bookingId, string userId)
        {
            var vendor = await GetVendorByUserIdAsync(userId);

            if (vendor == null)
            {
                return false;
            }

            var booking = await _context.Booking.FindAsync(bookingId);

            if (booking == null || booking.VendorId != vendor.Id)
            {
                return false;
            }

            booking.Status = "Confirmed";
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AddServiceAsync(VendorService service, string userId)
        {
            var vendor = await GetVendorByUserIdAsync(userId);

            if (vendor == null)
            {
                return false;
            }

            service.Vendor = vendor;
            service.IsDeleted = false;

            _context.VendorService.Add(service);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteServiceAsync(int serviceId, string userId)
        {
            var vendor = await GetVendorByUserIdAsync(userId);

            if (vendor == null)
            {
                return false;
            }

            var service = await _context.VendorService.FindAsync(serviceId);

            if (service == null || service.Vendor.Id != vendor.Id)
            {
                return false;
            }

            // Soft delete
            service.IsDeleted = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> GetBookingDetailsAsync(int bookingId, string userId)
        {
            var vendor = await GetVendorByUserIdAsync(userId);

            if (vendor == null)
            {
                return null;
            }

            var booking = await _context.Booking
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.VendorId == vendor.Id);

            if (booking == null)
            {
                return null;
            }

            // Get couple information
            var couple = await _context.Couple.FirstOrDefaultAsync(c => c.Id == booking.CoupleId);

            // Get service information
            var service = await _context.VendorService.FirstOrDefaultAsync(s => s.Id == booking.ServiceId);

            return new
            {
                booking.Id,
                booking.BookingDate,
                booking.ServiceDetails,
                booking.TotalAmount,
                booking.Status,
                booking.PaymentStatus,
                Couple = couple != null ? new
                {
                    couple.Id
                } : null,
                Service = service != null ? new
                {
                    service.Id,
                    service.ServiceName,
                    service.Price
                } : null,
                Payments = booking.Payments?.Select(p => new
                {
                    p.Id,
                    p.PaymentAmount,
                    p.PaymentDate,
                    p.PaymentMethod
                })
            };
        }

        public async Task<Vendor> CreateAsync(Vendor vendor)
        {
            _context.Vendor.Add(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<Vendor> UpdateAsync(Vendor vendor)
        {
            var existing = await _context.Vendor.FindAsync(vendor.Id);

            if (existing == null)
            {
                return null; // Or throw exception depending on how you want to handle it
            }

            // Update only the fields that can be changed
            existing.Name = vendor.Name;
            existing.Description = vendor.Description;
            existing.Pricing = vendor.Pricing;
            existing.Location = vendor.Location;
            existing.Rating = vendor.Rating;
            existing.ProfilePicture = vendor.ProfilePicture;
            existing.IsDeleted = vendor.IsDeleted;
            existing.CategoryId = vendor.CategoryId;
            existing.ContactEmail = vendor.ContactEmail;
            existing.Phone = vendor.Phone;
            existing.WebsiteUrl = vendor.WebsiteUrl;

            // Timestamp
            existing.UpdatedAt = DateTime.UtcNow;

            // Save changes
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<VendorService> GetServiceDetailsAsync(int serviceId, string userId)
        {

            var service = await _context.VendorService
                .FirstOrDefaultAsync(s => s.Id == serviceId);

            // Security check - verify the service belongs to this vendor
            if (service == null)
            {
                return null;
            }

            return service;
        }

        public async Task<bool> UpdateServiceAsync(VendorService updatedService, string userId)
        {

            // Find the original service
            var existingService = await _context.VendorService
                .Include(s => s.Vendor)
                .FirstOrDefaultAsync(s => s.Id == updatedService.Id);

            // Security check - verify the service belongs to this vendor
            if (existingService == null)
            {
                return false;
            }

            // Update service properties
            existingService.ServiceName = updatedService.ServiceName;
            existingService.Description = updatedService.Description;

            if (updatedService.Price <= 0) {
                existingService.Price = updatedService.Price;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
