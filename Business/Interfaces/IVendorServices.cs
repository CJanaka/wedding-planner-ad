using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface IVendorServices
    {
        Task<Vendor> GetById(int id);
    }
}
