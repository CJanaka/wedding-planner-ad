using System.Diagnostics;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Models.DTO;

namespace wedding_planer_ad.Business.Services
{
    public class HelperService
    {
        private readonly ICoupleDashboardService _coupleDashboardService;

        public HelperService(ICoupleDashboardService coupleDashboardService) {
            _coupleDashboardService = coupleDashboardService;
        }

        public async Task<BudgjetResponse> ModifyBudjet(bool isAddition, decimal amount, int coupleId) {

            var couple = await _coupleDashboardService.GetCoupleById(coupleId);

            BudgjetResponse response = new BudgjetResponse();

            if (couple == null) {
                response.Status = "failed";
                response.Message = "failure01";
                return response;
            }

            Debug.WriteLine("amount01 "+ amount);
            if (amount <= 0) {
                response.Status = "failed";
                response.Message = "failure02";
                return response;
            }

            var budgjet = couple.Budgets;

            if (budgjet == null) {
                response.Status = "failed";
                response.Message = "failure03";
                return response;
            }

            decimal remainBudgjet = budgjet.AllocatedAmount - budgjet.SpentAmount;
            if (!isAddition)
            {

                if (remainBudgjet < amount)
                {
                    response.Status = "failed";
                    response.Message = "Allocated budgjet exeeded!";
                    return response;
                }

                budgjet.SpentAmount += amount;
            }
            else {

                if (budgjet.SpentAmount < amount) {
                    response.Status = "failed";
                    response.Message = "Service fee and spent budgjet mismatched!";
                    return response;
                }
                //booking cancelation
                budgjet.SpentAmount -= amount;
            }

            await _coupleDashboardService.UpdateBudjetAsync(budgjet);

            response.Status = "success";
            response.Message = "Buajet Allocated!";
            return response;
        }
    }
}
