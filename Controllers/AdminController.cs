using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wedding_planer_ad.Business.Interfaces;

namespace wedding_planer_ad.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Planners()
        {
            var planners = await _userService.GetUsersInRoleAsync("Planner");
            return View(planners);
        }

        public async Task<IActionResult> Vendors()
        {
            var vendors = await _userService.GetUsersInRoleAsync("Vendor");
            return View(vendors);
        }

        public async Task<IActionResult> Couples()
        {
            var couples = await _userService.GetUsersInRoleAsync("Couple");
            return View(couples);
        }
    }
}
