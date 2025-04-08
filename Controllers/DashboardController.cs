using Microsoft.AspNetCore.Mvc;

namespace wedding_planer_ad.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
