using Microsoft.AspNetCore.Mvc;

namespace GymCenter.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
