using wedding_planer_ad.Models;
using wedding_planer_ad.Models.ViewModels;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface ICoupleDashboardService
    {
        Task<Couple> GetCoupleByUserId(String id);

        Task<IEnumerable<WeddingChecklist>> GetChecklistsByCoupleIdAsync(int coupleId);

        Task<VendorBookingViewModel> GetVendorsAsync(string searchString, int? categoryId, string location, string sortOrder);
        Task<VendorDetailsViewModel> GetVendorDetailsAsync(int id, string userId);
        Task<bool> BookVendorAsync(Booking booking);
        Task<List<Booking>> GetCoupleBookingsAsync(string userId);
    }
}
