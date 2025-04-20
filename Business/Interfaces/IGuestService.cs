using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface IGuestService
    {
        Task<IEnumerable<Guest>> GetGustsByCoupleId(int id);
        Task<Guest> CreateAsync(Guest guest);
        Task<Guest> UpdateAsync(Guest guest);

    }
}
