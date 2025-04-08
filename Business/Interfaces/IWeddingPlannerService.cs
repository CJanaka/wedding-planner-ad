using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface IWeddingPlannerService
    {
        Task<IEnumerable<WeddingPlanner>> GetAllAsync();
        Task<WeddingPlanner> GetByIdAsync(int id);
        Task<WeddingPlanner> CreateAsync(WeddingPlanner planner);
        Task<WeddingPlanner> UpdateAsync(WeddingPlanner planner);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Couple>> GetWeddingsByPlannerUserIdAsync(string plannerUserId);

        Task<IEnumerable<WeddingChecklist>> GetChecklistsByCoupleIdAsync(int coupleId);
       

        Task<IEnumerable<CoupleMember>> GetMembersByCoupleIdAsync(int coupleId);


        Task<IEnumerable<Guest>> GetGuestsByCoupleIdAsync(int coupleId);

        Task<IEnumerable<WeddingBudget>> GetBudgetsByCoupleIdAsync(int coupleId);

       Task<IEnumerable<WeddingTimeline>> GetTimelineByCoupleIdAsync(int coupleId);

         Task<IEnumerable<Booking>> GetBookingByCoupleId(int coupleId);
    }
}
