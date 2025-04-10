using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ICoupleDashboardService _coupleDashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(ICoupleDashboardService CoupleDashboardService,
            UserManager<ApplicationUser> userManager)
        {

            _coupleDashboardService = CoupleDashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                Debug.WriteLine("User not found!");
                return NotFound();
            }
            Debug.WriteLine("uid= "+ userId);

            var couple = await _coupleDashboardService.GetCoupleByUserId(userId);
            
            if (couple == null) { 
                Debug.WriteLine("couple not found!");

                return NotFound();
            }

            var checklists = await _coupleDashboardService.GetChecklistsByCoupleIdAsync(couple.Id);
            
            //var viewModel = new Couple
            //{
            //    Checklists = checklistItems.ToL
            //};

            {
                //var checklists = await _yourService.GetChecklistsByCoupleIdAsync(coupleId);

                //var viewModel = new Couple
                //{
                //    Checklists = checklists.ToList()
                //};

                var viewModel = couple;
                return View("Dashboard", viewModel);
            }
        }
    }
}
