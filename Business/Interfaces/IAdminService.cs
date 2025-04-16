using wedding_planer_ad.Models.ViewModels;

namespace wedding_planer_ad.Business.Interfaces
{
    public interface IAdminService
    {
        Task<AdminDashboardViewModel> GetDashboardData();

        Task<AdminReportViewModel> GetReportData();

        Task<AdminReportViewModel> GetWeeklyReportData();
        Task<AdminReportViewModel> GetMonthlyReportData();



    }
}
