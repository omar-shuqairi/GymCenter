using Microsoft.AspNetCore.Mvc;

namespace GymCenter.Controllers
{
    public class TrainerController : Controller
    {
        public IActionResult Home()
        {
            return View();
        }
    }
}
