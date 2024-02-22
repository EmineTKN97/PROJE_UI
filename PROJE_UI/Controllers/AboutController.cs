using Microsoft.AspNetCore.Mvc;

namespace PROJE_UI.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Mission()
        {
            return View();
        }
        public IActionResult Vision()
        {
            return View();
        }
        public IActionResult Who()
        {
            return View();
        }
    }
}
