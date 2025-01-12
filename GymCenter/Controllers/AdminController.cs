using GymCenter.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        public AdminController(ModelContext context)
        {
            _context = context;
        }
        public IActionResult Home()
        {

            ViewData["AdmimUserId"] = HttpContext.Session.GetInt32("AdminUserId");
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            ViewData["TotalMembers"] = _context.Members.Count();
            ViewData["TotalRevenue"] = _context.Payments.Sum(p => p.Amountpaid);
            ViewData["TotalActiveSubscriptions"] = _context.Members.Where(m => m.SubscriptionEnd > DateTime.Now).Count();
            ViewData["TotalSubscriptions"] = _context.Members.Where(m => m.SubscriptionStart != null && m.SubscriptionEnd != null).Count();
            return View();
        }
    }
}
