namespace wedding_planer_ad.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalWeddings { get; set; }
        public int CompletedWeddings { get; set; }
        public Dictionary<string, int> WeddingsPerMonth { get; set; }

        // 💡 New Stats You Can Add:
        public int TotalPlanners { get; set; }
        public int TotalVendors { get; set; }
        public int TotalCouples { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal AverageWeddingBudget { get; set; }
    }
}
