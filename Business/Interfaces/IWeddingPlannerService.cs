﻿using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;

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
       

        Task<IEnumerable<CoupleMemberDto>> GetMembersByCoupleIdAsync(int coupleId);


        Task<IEnumerable<Guest>> GetGuestsByCoupleIdAsync(int coupleId);

        Task<IEnumerable<WeddingBudget>> GetBudgetsByCoupleIdAsync(int coupleId);

       Task<IEnumerable<WeddingTimeline>> GetTimelineByCoupleIdAsync(int coupleId);

         Task<IEnumerable<Booking>> GetBookingByCoupleId(int coupleId);

        Task<WeddingTimeline> GetTimelineByIdAsync(int id);
        Task UpdateTimelineAsync(WeddingTimeline timeline);
        Task DeleteTimelineAsync(int id);

        Task AddTimelineAsync(WeddingTimeline timeline);

        Task<WeddingChecklist> GetChecklistByIdAsync(int id);

        Task UpdateChecklistAsync(WeddingChecklist checklist);

        Task DeleteChecklistAsync(int id);

        Task AddChecklistTaskAsync(WeddingChecklist checklist);

        Task<IEnumerable<BookingVendorDTO>> GetBookingByCoupleIdWithVendor(int coupleId);



    }
}
