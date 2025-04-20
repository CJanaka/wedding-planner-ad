using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Business.Services;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;

namespace wedding_planer_ad.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICoupleDashboardService _coupleDashboardService;
        private readonly IWeddingPlannerService _plannerService;

        public AccountController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ICoupleDashboardService coupleDashboardService,
            IWeddingPlannerService plannerService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _coupleDashboardService = coupleDashboardService;
            _plannerService = plannerService;
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl = "/Identity/Account/Login")
        {
            await _signInManager.SignOutAsync();

            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Get all active wedding planners to display in the popup
            var planners = _plannerService.GetAllAsync();
            ViewBag.Planners = planners;

            return View(new NewCoupleDto());
        }

        [HttpPost]
        public async Task<IActionResult> Register(NewCoupleDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Couple");
                await _signInManager.SignInAsync(user, isPersistent: false);



                var couple = new Couple
                {
                    UserId = user.Id,
                    User = user
                };

                try
                {
                    couple = await _coupleDashboardService.CreateAsync(couple);

                    var budgjet = new WeddingBudget
                    {
                        CoupleId = couple.Id,
                        AllocatedAmount = 0,
                        IsDeleted = false,
                        SpentAmount = 0,
                        PaymentDate = DateTime.Now,
                    };

                    await _coupleDashboardService.CreateBudjetAsync(budgjet);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    throw new InvalidDataException(ex.Message);
                }

                return Redirect("/Identity/Account/Login"); // Or wherever you want to redirect after successful registration
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(dto);
        }
    }
}
