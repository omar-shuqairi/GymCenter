using Microsoft.AspNetCore.Mvc;

namespace GymCenter.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Home()
        {
            return View();
        }
    }
}
