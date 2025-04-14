namespace wedding_planer_ad.Models.ViewModels
{
    public class WeddingPlannerDashboardViewModel
    {
        public int TotalWeddings { get; set; }
        public int CompletedWeddings { get; set; }
        public int UpcomingWeddings { get; set; }
        public decimal TotalBudget { get; set; }

        // For chart
        public List<int> CompletedWeddingsPerMonth { get; set; }
        public List<int> UpcomingWeddingsPerMonth { get; set; }
    }


}
