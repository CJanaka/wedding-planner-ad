using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Business.Services;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace wedding_planer_ad.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ICoupleDashboardService _coupleDashboardService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWeddingPlannerService _plannerService;
        private readonly IUserService _userService;
        private readonly HelperService _helperService;


        public DashboardController(ICoupleDashboardService CoupleDashboardService,
            UserManager<ApplicationUser> userManager,
            IWeddingPlannerService plannerService,
            IUserService userService, HelperService helperService)
        {

            _coupleDashboardService = CoupleDashboardService;
            _userManager = userManager;
            _plannerService = plannerService;
            _userService = userService;
            _helperService = helperService;
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

            {
                var viewModel = couple;
                return View("Dashboard", viewModel);
            }
        }

        public async Task<IActionResult> MyBookings()
        {
            string userId = _userManager.GetUserId(User);

            var bookings = await _coupleDashboardService.GetCoupleBookingsAsync(userId);

            return View(bookings);
        }

        public async Task<IActionResult> Planner()
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                Debug.WriteLine("User not found!");
                return NotFound();
            }

            var couple = await _coupleDashboardService.GetCoupleByUserId(userId);

            if (couple == null)
            {
                Debug.WriteLine("couple not found!");
                return NotFound();
            }

            var planners = await _userManager.GetUsersInRoleAsync("Planner");

            var plan = await _plannerService.GetByCoupleId(couple.Id);

            var viewModel = new PlannerDetailsViewModel();
            if (plan != null)
            {
                var selectedPlanner = await _userService.GetUserByIdAsync(plan.PlannerUserId);
                viewModel.selectedWeddingPlanner = selectedPlanner;
            }

            viewModel.CoupleId = couple.Id;
            viewModel.Planners = planners.ToList();

            return View("Planner", viewModel);
        }
        public async Task<IActionResult> Details(string id)
        {
            var planner = await _userService.GetUserByIdAsync(id);
            if (planner == null)
            {
                return NotFound();
            }

            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                Debug.WriteLine("User not found!");
                return NotFound();
            }

            var couple = await _coupleDashboardService.GetCoupleByUserId(userId);

            if (couple == null)
            {
                Debug.WriteLine("couple not found!");
                return NotFound();
            }

            List<DateTime> bookedDates = new List<DateTime>
            {
                DateTime.Now.Date,                    // Today
                DateTime.Now.Date.AddDays(1),         // Tomorrow
                DateTime.Now.Date.AddDays(3),         // 3 days from now
                DateTime.Now.Date.AddDays(5),         // 5 days from now
                DateTime.Now.Date.AddDays(10),        // 10 days from now
                DateTime.Now.Date.AddDays(15)
            };
            var viewModel = new PlannerDetailsViewModel
            {
                selectedWeddingPlanner = planner,
                CoupleId = couple.Id,
                BookedDates = bookedDates
            };

            return View("PlannerDetails", viewModel);
        }

        // POST: WeddingPlanner/Book
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BookPlanner(string plannerId, int coupleId, DateTime bookingDate)
        {

            var planer = new WeddingPlanner { 
                PlannerUserId = plannerId,
                CoupleId = coupleId,
                AssignedDate = bookingDate,
                IsDeleted = false
            };

            var success = await _plannerService.CreateAsync(planer);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to book the planner for the selected date. Please try another date.";
                return RedirectToAction(nameof(Details), new { id = plannerId });
            }

            TempData["SuccessMessage"] = "Booking successful! Your booking has been confirmed for " + bookingDate.ToString("dddd, MMMM d, yyyy");
            
            return RedirectToAction(nameof(Planner));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UpdateBooking(int id, DateTime bookingDate)
        {


            var booking = new Booking
            {
                BookingDate = bookingDate,
                Id = id,
                IsDeleted = false
            };

            var success = await _coupleDashboardService.UpdateBookingAsync(booking);

            if (!success)
            {
                TempData["ErrorMessage"] = "Unable to book the planner for the selected date. Please try another date.";
                return RedirectToAction(nameof(MyBookings));
            }

            TempData["SuccessMessage"] = "Booking successful! Your booking has been confirmed for " + bookingDate.ToString("dddd, MMMM d, yyyy");

            return RedirectToAction(nameof(MyBookings));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AddReview(int id, int review)
        {


            var booking = new Booking
            {
                Id = id,
                IsDeleted = false
            };

            if (review > 0)
            {
                var reviews = new List<Review>();
                var rvw = new Review
                {
                    IsDeleted = false,
                    Rating = review,
                };
                reviews.Add(rvw);

                booking.Reviews = reviews;
            }

            var success = await _coupleDashboardService.UpdateBookingAsync(booking);

            if (!success)
            {
                TempData["ErrorMessage"] = "Reviewing failed!";
                return RedirectToAction(nameof(MyBookings));
            }

            TempData["SuccessMessage"] = "Thanks for your review!";

            return RedirectToAction(nameof(MyBookings));
        }

        [HttpPost]
        public async Task<IActionResult> AddChecklist(WeddingChecklist checklist)
        {
            checklist.AssignedTo = "";
            if (ModelState.IsValid)
            {
                await _plannerService.AddChecklistTaskAsync(checklist);
                return RedirectToAction("Dashboard");
            }
            foreach (var modelState in ModelState)
            {
                foreach (var error in modelState.Value.Errors)
                {
                    Console.WriteLine($"Error in {modelState.Key}: {error.ErrorMessage}");
                }
            }
            return View(checklist);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateChecklist(WeddingChecklist checklist)
        {
            var existing = await _plannerService.GetChecklistByIdAsync(checklist.Id);
            Debug.WriteLine("checklist.IsCompleted "+ checklist.IsCompleted);
            existing.DueDate = checklist.DueDate;
            existing.IsCompleted = checklist.IsCompleted;
            existing.TaskName = checklist.TaskName;

            await _plannerService.UpdateChecklistAsync(existing);
            return RedirectToAction("Dashboard");
        }

        // POST: Delete Checklist Item
        [HttpPost]
        public async Task<IActionResult> DeleteChecklist(int id)
        {
            var item = await _plannerService.GetChecklistByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            int coupleId = item.CoupleId;
            await _plannerService.DeleteChecklistAsync(id);
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> AddTimeline(WeddingTimeline timeline)
        {
            if (!ModelState.IsValid)
                return View(timeline);

            try
            {
                
                await _plannerService.AddTimelineAsync(timeline);
                TempData["SuccessMessage"] = "Timeline added successfully!";
                return RedirectToAction("Dashboard");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("AddTimeline Exception", ex.Message);
                TempData["ErrorMessage"] = "Time line adding failed!";

                return RedirectToAction("Dashboard");

            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTimeline(WeddingTimeline timeline)
        {
            var existingTimeline = await _plannerService.GetTimelineByIdAsync(timeline.Id);

            if(timeline.EventDescription != null)
                existingTimeline.EventDescription = timeline.EventDescription;
            
            if (timeline.EventName != null)
                existingTimeline.EventName = timeline.EventName;

            existingTimeline.EventTime = timeline.EventTime;

            await _plannerService.UpdateTimelineAsync(existingTimeline);

            TempData["SuccessMessage"] = "Time line updated!";
            return RedirectToAction("Dashboard");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTimeline(int id)
        {
            var timeline = await _plannerService.GetTimelineByIdAsync(id);
            if (timeline == null)
            {
                TempData["ErrorMessage"] = "Time line remove failed!";
                return RedirectToAction("Dashboard");
            }

            await _plannerService.DeleteTimelineAsync(id);

            TempData["SuccessMessage"] = "Time line removed!";
            return RedirectToAction("Dashboard");
        }

        //UpdateBudjet
        [HttpPost]
        public async Task<IActionResult> UpdateBudjet(WeddingBudget weddingBudget)
        {
            var existing = await _coupleDashboardService.GetBudgetsByIdAsync(weddingBudget.Id);
            Debug.WriteLine("Budget updated AllocatedAmount: " + weddingBudget.AllocatedAmount);

            if (existing == null) {
                Debug.WriteLine("Budget record not found " + weddingBudget.Id);
                return new NotFoundResult();
            }

            if (weddingBudget.AllocatedAmount > 0)
            {
                try { 

                    existing.AllocatedAmount = weddingBudget.AllocatedAmount;
                    await _coupleDashboardService.UpdateBudjetAsync(existing);
                    TempData["SuccessMessage"] = "Budget updated successfully!";

                }catch(Exception e) {
                    Debug.WriteLine("Budget updated failed: " + e.Message);
                    TempData["ErrorMessage"] = "Budget updated failed!";
                }
            }
            return RedirectToAction("Dashboard");
        }

    }

}
