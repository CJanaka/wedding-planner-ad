namespace wedding_planer_ad.Models.ViewModels
{
    public class AdminReportViewModel
    {
        public int TotalWeddings { get; set; }
        public int CompletedWeddings { get; set; }
        public int TotalPlanners { get; set; }
        public int TotalVendors { get; set; }
        public int TotalCouples { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal AverageWeddingBudget { get; set; }

        public int WeddingsThisWeek { get; set; }
        public int WeddingsThisMonth { get; set; }
        public int BookingsThisWeek { get; set; }
        public int BookingsThisMonth { get; set; }

    }
}
