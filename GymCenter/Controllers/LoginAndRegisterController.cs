using Microsoft.AspNetCore.Mvc;

namespace GymCenter.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }
    }
}
