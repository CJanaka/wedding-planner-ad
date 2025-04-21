using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Business.Services;
using wedding_planer_ad.Models;
using wedding_planer_ad.Utils;

namespace wedding_planer_ad.Controllers
{
    public class VendorController : Controller
    {
        private readonly ICoupleDashboardService _coupleDashboardService;
        private readonly IVendorServices _vendorService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HelperService _helperService;

        public VendorController(ICoupleDashboardService CoupleDashboardService,
            UserManager<ApplicationUser> userManage, IVendorServices vendorService,
            HelperService helperService) {


            _coupleDashboardService = CoupleDashboardService;
            _userManager = userManage;
            _vendorService = vendorService;
            _helperService = helperService;
        }
        
        public async Task<IActionResult> VendorListView(string searchString, int? categoryId, string location, string sortOrder)
        {
            var viewModel = await _coupleDashboardService.GetVendorsAsync(searchString, categoryId, location, sortOrder);
            return View(viewModel);
        }

        public async Task<IActionResult> VendorDetailView(int id)
        {

            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                Debug.WriteLine("User not found!");
                return NotFound();
            }

            Debug.WriteLine("usr id "+ userId);

            var viewModel = await _coupleDashboardService.GetVendorDetailsAsync(id, userId);

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

            var vendor = await  _vendorService.GetById(booking.VendorId);

            if (vendor == null)
            {
                Debug.WriteLine("vendor not found!");
                return NotFound();
            }

            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Count > 0)
                {
                    Debug.WriteLine($"Property: {state.Key}, Errors: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
                }
            }

            booking.PaymentStatus = Util.PAID_STATUS;
            booking.VendorId = vendor.Id;

            if (ModelState.IsValid)
            {
                //Adjust allocated budgjet according to the booking fee
                var response = await _helperService.ModifyBudjet(false, booking.TotalAmount, booking.CoupleId);

                if (response.Status.Equals("failed")) {

                    TempData["ErrorMessage"] = response.Message;
                    return RedirectToAction(nameof(MyBookings));
                }

                bool result = await _coupleDashboardService.BookVendorAsync(booking);

                if (result)
                {

                    TempData["SuccessMessage"] = "Booking request sent successfully!";
                    return RedirectToAction(nameof(MyBookings));
                }
            }
         
            var viewModel = await _coupleDashboardService.GetVendorDetailsAsync(booking.VendorId, User.Identity.Name);
            viewModel.Booking = booking;

            return View("VendorDetailView", viewModel);
        }

        public async Task<IActionResult> MyBookings()
        {
            string userId = _userManager.GetUserId(User);

            var bookings = await _coupleDashboardService.GetCoupleBookingsAsync(userId);
            Debug.WriteLine($"My bookings: {bookings}");
            Debug.WriteLine("My bookingscoun5t: "+ bookings.Count);

            return View("~/Views/Dashboard/MyBookings.cshtml", bookings);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CancelBooking(int id, int coupleId, decimal totalAmount)
        {

            var booking = new Booking
            {
                Id = id,
                IsDeleted = true
            };

            //Adjust allocated budgjet according to the booking fee
            var response = await _helperService.ModifyBudjet(true, totalAmount, coupleId);

            if (response.Status.Equals("failed"))
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction(nameof(MyBookings));
            }

            var success = await _coupleDashboardService.UpdateBookingAsync(booking);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to remove booking.";
                return RedirectToAction(nameof(MyBookings));
            }

            TempData["SuccessMessage"] = "Booking removed!";

            return RedirectToAction(nameof(MyBookings));
        }

        public async Task<IActionResult> Vendor()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            Debug.WriteLine("pop here01");

            var viewModel = await _vendorService.GetDashboardDataAsync(userId);

            if (viewModel == null)
            {
                Debug.WriteLine("pop here02");
                return NotFound();
            }

            return View("Vendor",viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmBooking(int bookingId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _vendorService.ConfirmBookingAsync(bookingId, userId);

            if (!result)
            {
                return NotFound("Booking not found or you don't have permission to confirm it");
            }

            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> AddService(VendorService service)
        {
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Debug.WriteLine($"Model error for '{key}': {error.ErrorMessage}");
                    }
                }
                // Return to dashboard with validation errors
                return RedirectToAction("Vendor");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _vendorService.AddServiceAsync(service, userId);

            if (!result)
            {
                return NotFound("Vendor not found");
            }

            return RedirectToAction("Vendor");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteService(int serviceId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _vendorService.DeleteServiceAsync(serviceId, userId);

            if (!result)
            {
                return NotFound("Service not found or you don't have permission to delete it");
            }

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var bookingDetails = await _vendorService.GetBookingDetailsAsync(id, userId);

            if (bookingDetails == null)
            {
                return NotFound("Booking not found or you don't have permission to view it");
            }

            return Json(bookingDetails);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateService(VendorService service)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors if needed
                return RedirectToAction("Vendor");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _vendorService.UpdateServiceAsync(service, userId);

            if (!result)
            {
                return NotFound("Service not found or you don't have permission to update it");
            }

            return RedirectToAction("Vendor");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int id, string status)
        {


            var booking = new Booking
            {
                Id = id,
                Status = status,
                IsDeleted = false
            };

            var success = await _coupleDashboardService.UpdateBookingAsync(booking);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to updated booking status";
                return RedirectToAction(nameof(Vendor));
            }

            TempData["SuccessMessage"] = "Booking status updated successful!";

            return RedirectToAction(nameof(Vendor));
        }
    }
}
