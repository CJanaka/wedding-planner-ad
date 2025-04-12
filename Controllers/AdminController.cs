using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;

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

        public async Task<IActionResult> PlannerDetails(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var dto = new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> PlannerDetails(AdminUserDto dto, string? delete)
        {
            if (!string.IsNullOrEmpty(delete))
            {
                Console.WriteLine("Deleting..........");
                await _userService.DeleteUserAsync(dto.Id);
                return RedirectToAction("Planners");
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model not valid..........");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                return View(dto);
            }

            await _userService.UpdateUserFromDTOAsync(dto);
            return RedirectToAction("Planners");
        }

        [HttpGet]
        public IActionResult AddPlanner()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPlanner(NewPlannerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _userService.CreatePlannerAsync(dto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(dto);
            }

            return RedirectToAction("Planners");
        }

        [HttpGet]
        public async Task<IActionResult> VendorDetails(string id)
        {
            var vendor = await _userService.GetUserByIdAsync(id);
            if (vendor == null) return NotFound();

            var dto = new AdminUserDto
            {
                Id = vendor.Id,
                Email = vendor.Email,
                FirstName = vendor.FirstName,
                LastName = vendor.LastName,
                PhoneNumber = vendor.PhoneNumber
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> VendorDetails(AdminUserDto dto, string? delete)
        {
            if (!string.IsNullOrEmpty(delete))
            {
                await _userService.DeleteUserAsync(dto.Id);
                return RedirectToAction("Vendors");
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _userService.UpdateUserFromDTOAsync(dto);
            return RedirectToAction("Vendors");
        }

        [HttpGet]
        public IActionResult AddVendor()
        {
            return View(new AdminUserDto());
        }
       

        [HttpPost]
        public async Task<IActionResult> AddVendor(NewVendorDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _userService.CreateVendorAsync(dto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(dto);
            }

            return RedirectToAction("Couples");
        }

        [HttpGet]
        public IActionResult AddCouple()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCouple(NewCoupleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _userService.CreateCoupleAsync(dto);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(dto);
            }

            return RedirectToAction("Couples");
        }




        public async Task<IActionResult> CoupleDetails(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var dto = new AdminUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CoupleDetails(AdminUserDto dto, string? delete)
        {
            if (!string.IsNullOrEmpty(delete))
            {
                await _userService.DeleteUserAsync(dto.Id);
                return RedirectToAction("Couples");
            }
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Model not valid..........");
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                return View(dto);
            }

            await _userService.UpdateUserFromDTOAsync(dto);
            return RedirectToAction("Couples");
        }


    }
}
