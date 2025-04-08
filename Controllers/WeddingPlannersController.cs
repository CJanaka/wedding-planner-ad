using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Models;
using wedding_planer_ad.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace wedding_planer_ad.Controllers
{
    public class WeddingPlannersController : Controller
    {
        private readonly IWeddingPlannerService _plannerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public WeddingPlannersController(
         IWeddingPlannerService plannerService,
         UserManager<ApplicationUser> userManager)
        {
            _plannerService = plannerService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var planners = await _plannerService.GetAllAsync();
            return View(planners);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var planner = await _plannerService.GetByIdAsync(id.Value);
            if (planner == null) return NotFound();

            return View(planner);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PlannerUserId,CoupleId,AssignedDate,IsDeleted")] WeddingPlanner weddingPlanner)
        {
            if (!ModelState.IsValid) return View(weddingPlanner);

            await _plannerService.CreateAsync(weddingPlanner);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var planner = await _plannerService.GetByIdAsync(id.Value);
            if (planner == null) return NotFound();

            return View(planner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PlannerUserId,CoupleId,AssignedDate,IsDeleted")] WeddingPlanner weddingPlanner)
        {
            if (id != weddingPlanner.Id) return NotFound();

            if (!ModelState.IsValid) return View(weddingPlanner);

            try
            {
                await _plannerService.UpdateAsync(weddingPlanner);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var planner = await _plannerService.GetByIdAsync(id.Value);
            if (planner == null) return NotFound();

            return View(planner);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _plannerService.DeleteAsync(id);
            }
            catch (ResourceNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MyWeddings()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null) {
                var weddings = await _plannerService.GetWeddingsByPlannerUserIdAsync(userId);
                return View(weddings);
            }

            return View();
            
        }

        public async Task<IActionResult> ViewMembers(int coupleId)
        {
            var members = await _plannerService.GetMembersByCoupleIdAsync(coupleId);
            return View(members);
        }

        public async Task<IActionResult> ViewChecklists(int coupleId)
        {
            var checklists = await _plannerService.GetChecklistsByCoupleIdAsync(coupleId);
            return View(checklists);
        }

        public async Task<IActionResult> ViewGuests(int coupleId)
        {
            var guests = await _plannerService.GetGuestsByCoupleIdAsync(coupleId);
            return View(guests);
        }

        //public async Task<IActionResult> ViewWeddingBudget(int coupleId)
        //{
        //    var guests = await _plannerService.GetBudgetsByCoupleIdAsync(coupleId);
        //    return View(guests);
        //}

        public async Task<IActionResult> ViewWeddingTimeline(int coupleId)
        {
            var guests = await _plannerService.GetTimelineByCoupleIdAsync(coupleId);
            return View(guests);
        }

        public async Task<IActionResult> ViewWeddingBooking(int coupleId)
        {
            var guests = await _plannerService.GetBookingByCoupleId(coupleId);
            return View(guests);
        }


    }
}
