using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Business.Services;
using wedding_planer_ad.Data;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.ViewModels;

namespace wedding_planer_ad.Controllers
{
    [Authorize(Roles = "Couple")]
    public class GuestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IGuestService _guestService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICoupleDashboardService _coupleDashboardService;


        public GuestController(ApplicationDbContext context, IGuestService guestService,
             UserManager<ApplicationUser> userManager, ICoupleDashboardService coupleDashboardService)
        {
            _context = context;
            _guestService = guestService;
            _userManager = userManager;
            _coupleDashboardService = coupleDashboardService;

        }

        public async Task<IActionResult> Guest()
        {
            var userId = _userManager.GetUserId(User);
            var couple = await _coupleDashboardService.GetCoupleByUserId(userId);

            if (couple == null)
            {
                Debug.WriteLine("couple not found!");
                return NotFound();
            }
            var existingGuests = await _guestService.GetGustsByCoupleId(couple.Id);

            var ViewModel = new GuestViewModel
            {
                CoupleId = couple.Id,
                Guests = existingGuests.ToList()
            };
            return View("~/Views/Dashboard/Guest.cshtml", ViewModel);
        }

        // Assuming you use authentication and can get the current user’s coupleId
        public async Task<IActionResult> Create(Guest guest)
        {
            // Example: get coupleId from logged-in user (adjust this based on your auth setup)
            var userId = _userManager.GetUserId(User);
            var couple = await _coupleDashboardService.GetCoupleByUserId(userId);

            if (couple == null)
            {
                Debug.WriteLine("couple not found!");
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                Debug.WriteLine("valid guest! "+guest.CoupleId);

                guest.IsDeleted = false;
                guest = await _guestService.CreateAsync(guest);

                //_context.Guests.Add(guest);
                //await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Guest));
            }

            ViewBag.CoupleId = couple.Id;

            // Pass existing guests for the couple to the view
            var existingGuests = _guestService.GetGustsByCoupleId(couple.Id);

            ViewBag.ExistingGuests = existingGuests;

            return View();
        } 
        
        public async Task<IActionResult> Update(Guest guest)
        {
            // Example: get coupleId from logged-in user (adjust this based on your auth setup)
            var userId = _userManager.GetUserId(User);
            var couple = await _coupleDashboardService.GetCoupleByUserId(userId);

            if (couple == null)
            {
                Debug.WriteLine("couple not found!");
                return NotFound();
            }

            if (ModelState.IsValid)
            {


                guest = await _guestService.UpdateAsync(guest);

                return RedirectToAction(nameof(Guest));
            }
            else {
                Debug.WriteLine("invalid guest! " + guest.CoupleId);
            }

            ViewBag.CoupleId = couple.Id;

            // Pass existing guests for the couple to the view
            var existingGuests = _guestService.GetGustsByCoupleId(couple.Id);

            ViewBag.ExistingGuests = existingGuests;

            return View();
        }

        public IActionResult GuestList(int coupleId)
        {
            var existingGuests = _guestService.GetGustsByCoupleId(coupleId);

            return View(existingGuests);
        }
    }
}
