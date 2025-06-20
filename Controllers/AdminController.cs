﻿using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wedding_planer_ad.Business.Interfaces;
using wedding_planer_ad.Business.Services;
using wedding_planer_ad.Models;
using wedding_planer_ad.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace wedding_planer_ad.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IWeddingPlannerService _weddingService;
        private readonly IAdminService _adminService;
        private readonly IVendorServices _vendorServices;


        public AdminController(IUserService userService, IWeddingPlannerService weddingService,
            IAdminService adminService, IVendorServices vendorServices)
        {
            _userService = userService;
            _weddingService = weddingService;
            _adminService = adminService;
            _vendorServices = vendorServices;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _adminService.GetDashboardData();
            return View(model);
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
            return View(new NewVendorDto());
        }
       

        [HttpPost]
        public async Task<IActionResult> AddVendor(NewVendorDto dto)
        {
            Debug.WriteLine("venerror00 "+ ModelState.IsValid);

            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Debug.WriteLine($"Model error for '{key}': {error.ErrorMessage}");
                }
            }
            if (!ModelState.IsValid)
            {
                return View(dto);
            }
            try { 
            
                var result = await _userService.CreateVendorAsync(dto);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(dto);
                }

                var user = await _userService.GetByEmail(dto.Email);

                var vendor = new Vendor {
                    UserId = user.Id,
                    Name = dto.FirstName,
                    ContactEmail = dto.Email,
                    Phone = dto.PhoneNumber,
                    Pricing = 0,
                    Rating = 0,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,

                };

                await _vendorServices.CreateAsync(vendor);
            }
            catch (Exception e) {
                Debug.WriteLine("venerror3 ");

                throw new Exception("Vendor creation failed: "+e.Message);
            }
            return RedirectToAction("Vendor");
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

        public async Task<IActionResult> Reports()
        {
            var model = await _adminService.GetReportData();
            return View(model);
        }

        public async Task<IActionResult> DownloadPdfReport()
        {
            var model = await _adminService.GetReportData();

            using (var memoryStream = new MemoryStream())
            {
                var doc = new Document(PageSize.A4);
                PdfWriter.GetInstance(doc, memoryStream);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                doc.Add(new Paragraph("Wedding Planner Admin Report", titleFont));
                doc.Add(new Paragraph("Generated: " + DateTime.Now.ToString("f")));
                doc.Add(new Paragraph(" "));

                var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                doc.Add(new Paragraph($"Total Weddings: {model.TotalWeddings}", bodyFont));
                doc.Add(new Paragraph($"Completed Weddings: {model.CompletedWeddings}", bodyFont));
                doc.Add(new Paragraph($"Total Planners: {model.TotalPlanners}", bodyFont));
                doc.Add(new Paragraph($"Total Vendors: {model.TotalVendors}", bodyFont));
                doc.Add(new Paragraph($"Total Couples: {model.TotalCouples}", bodyFont));
                doc.Add(new Paragraph($"Total Bookings: {model.TotalBookings}", bodyFont));
                doc.Add(new Paragraph($"Total Budget: Rs. {model.TotalBudget:N2}", bodyFont));
                doc.Add(new Paragraph($"Average Wedding Budget: Rs. {model.AverageWeddingBudget:N2}", bodyFont));

                doc.Close();

                byte[] pdfBytes = memoryStream.ToArray();
                return File(pdfBytes, "application/pdf", "AdminReport.pdf");
            }
        }

        public async Task<IActionResult> DownloadWeeklyReport()
        {
            var model = await _adminService.GetWeeklyReportData();

            using var memoryStream = new MemoryStream();
            var doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            doc.Add(new Paragraph("Weekly Wedding Report", titleFont));
            doc.Add(new Paragraph("Generated: " + DateTime.Now.ToString("f")));
            doc.Add(new Paragraph(" "));

            doc.Add(new Paragraph($"Total Weddings: {model.TotalWeddings}", bodyFont));
            doc.Add(new Paragraph($"Completed Weddings: {model.CompletedWeddings}", bodyFont));
            doc.Add(new Paragraph($"Total Bookings: {model.TotalBookings}", bodyFont));
            doc.Add(new Paragraph($"Total Budget: Rs. {model.TotalBudget:N2}", bodyFont));
            doc.Add(new Paragraph($"Average Wedding Budget: Rs. {model.AverageWeddingBudget:N2}", bodyFont));

            doc.Close();
            return File(memoryStream.ToArray(), "application/pdf", "WeeklyReport.pdf");
        }

        public async Task<IActionResult> DownloadMonthlyReport()
        {
            var model = await _adminService.GetMonthlyReportData();

            using var memoryStream = new MemoryStream();
            var doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            doc.Add(new Paragraph("Monthly Wedding Report", titleFont));
            doc.Add(new Paragraph("Generated: " + DateTime.Now.ToString("f")));
            doc.Add(new Paragraph(" "));

            doc.Add(new Paragraph($"Total Weddings: {model.TotalWeddings}", bodyFont));
            doc.Add(new Paragraph($"Completed Weddings: {model.CompletedWeddings}", bodyFont));
            doc.Add(new Paragraph($"Total Bookings: {model.TotalBookings}", bodyFont));
            doc.Add(new Paragraph($"Total Budget: Rs. {model.TotalBudget:N2}", bodyFont));
            doc.Add(new Paragraph($"Average Wedding Budget: Rs. {model.AverageWeddingBudget:N2}", bodyFont));

            doc.Close();
            return File(memoryStream.ToArray(), "application/pdf", "MonthlyReport.pdf");
        }
    }

}

