using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface ICoupleDashboardService
    {
        Task<Couple> GetCoupleByUserId(String id);

        Task<IEnumerable<WeddingChecklist>> GetChecklistsByCoupleIdAsync(int coupleId);
    }
}
