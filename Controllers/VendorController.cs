using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Models;

namespace wedding_planer_ad.Controllers
{
    public class VendorController : Controller
    {
        private readonly ICoupleDashboardService _coupleDashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendorController(ICoupleDashboardService CoupleDashboardService,
            UserManager<ApplicationUser> userManage) {


            _coupleDashboardService = CoupleDashboardService;
            _userManager = userManage;
        }
        
        public async Task<IActionResult> VendorListView(string searchString, int? categoryId, string location, string sortOrder)
        {
            var viewModel = await _coupleDashboardService.GetVendorsAsync(searchString, categoryId, location, sortOrder);
            return View(viewModel);
        }

        public async Task<IActionResult> VendorDetailView(int id)
        {
            var viewModel = await _coupleDashboardService.GetVendorDetailsAsync(id, User.Identity.Name);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(Booking booking)
        {
            if (ModelState.IsValid)
            {
                bool result = await _coupleDashboardService.BookVendorAsync(booking);

                if (result)
                {
                    TempData["SuccessMessage"] = "Booking request sent successfully!";
                    return RedirectToAction(nameof(MyBookings));
                }
            }

            // If we got this far, something failed, redisplay form
            var viewModel = await _coupleDashboardService.GetVendorDetailsAsync(booking.VendorId, User.Identity.Name);
            viewModel.Booking = booking; // Use the booking with validation errors

            return View("Details", viewModel);
        }

        public async Task<IActionResult> MyBookings()
        {
            var bookings = await _coupleDashboardService.GetCoupleBookingsAsync(User.Identity.Name);
            return View(bookings);
        }
    }
}
