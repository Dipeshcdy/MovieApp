using Microsoft.AspNetCore.Mvc;

namespace MovieApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private IActionResult HandleAuthorization()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Login", "Authentication", new { area = "User" });
            }

            if (!User.IsInRole("Admin"))
            {
                TempData["error"] = "Not Authorized";
                return RedirectToAction("Index", "Home", new { area = "User" });
            }

            return null;
        }
        public IActionResult Index()
        {
            IActionResult authResult = HandleAuthorization();
            if (authResult != null)
            {
                return authResult;
            }
            return View();
        }
    }
}
