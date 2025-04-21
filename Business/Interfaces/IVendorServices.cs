using wedding_planer_ad.Models;
using wedding_planer_ad.Models.ViewModels;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface IVendorServices
    {
        Task<Vendor> GetById(int id);
        Task<Vendor> CreateAsync(Vendor vendor);
        Task<Vendor> UpdateAsync(Vendor vendor);

        Task<Vendor> GetVendorByUserIdAsync(string userId);
        Task<VendorDashboardViewModel> GetDashboardDataAsync(string userId);
        Task<bool> ConfirmBookingAsync(int bookingId, string userId);
        Task<bool> AddServiceAsync(VendorService service, string userId);
        Task<bool> DeleteServiceAsync(int serviceId, string userId);
        Task<object> GetBookingDetailsAsync(int bookingId, string userId);

        Task<VendorService> GetServiceDetailsAsync(int serviceId, string userId);
        Task<bool> UpdateServiceAsync(VendorService service, string userId);
    }
}
