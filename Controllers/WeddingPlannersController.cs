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
            ViewBag.CoupleId = coupleId;
            var guests = await _plannerService.GetTimelineByCoupleIdAsync(coupleId);
            return View(guests);
        }

        public async Task<IActionResult> ViewWeddingBooking(int coupleId)
        {
            var guests = await _plannerService.GetBookingByCoupleId(coupleId);
            return View(guests);
        }

        public async Task<IActionResult> TimelineDetails(int id)
        {
            var item = await _plannerService.GetTimelineByIdAsync(id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTimeline(WeddingTimeline timeline, bool delete = false)
        {
            if (delete)
            {
                var existingTimeline = await _plannerService.GetTimelineByIdAsync(timeline.Id);
                if (existingTimeline == null)
                {
                    return NotFound();
                }

                // Delete timeline logic
                await _plannerService.DeleteTimelineAsync(timeline.Id);

                return RedirectToAction("ViewWeddingTimeline", new { coupleId = timeline.CoupleId });
            }

            // Update timeline logic
            await _plannerService.UpdateTimelineAsync(timeline);

            return RedirectToAction("ViewWeddingTimeline", new { coupleId = timeline.CoupleId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTimeline(int id)
        {
            var timeline = await _plannerService.GetTimelineByIdAsync(id);
            if (timeline == null)
            {
                return NotFound();
            }

            int coupleId = timeline.CoupleId;

            await _plannerService.DeleteTimelineAsync(id);

            return RedirectToAction("ViewWeddingTimeline", new { coupleId = coupleId });
        }


        [HttpGet]
        public IActionResult AddTimeline(int coupleId)
        {
            var model = new WeddingTimeline
            {
                CoupleId = coupleId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTimeline(WeddingTimeline timeline)
        {
            if (!ModelState.IsValid)
                return View(timeline);

            try
            {
                await _plannerService.AddTimelineAsync(timeline);
                return RedirectToAction("ViewWeddingTimeline", new { coupleId = timeline.CoupleId });
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("CoupleId", ex.Message);
                return View(timeline); 
            }
        }




    }
}
