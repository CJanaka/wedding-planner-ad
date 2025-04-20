namespace wedding_planer_ad.Models.ViewModels
{
    public class PlannerDetailsViewModel
    {
        //public ApplicationUser Planner { get; set; }
        public List<ApplicationUser> Planners { get; set; }
        public int CoupleId { get; set; }
        public ApplicationUser selectedWeddingPlanner { get; set; }
        public List<DateTime> BookedDates { get; set; }
    }
}
