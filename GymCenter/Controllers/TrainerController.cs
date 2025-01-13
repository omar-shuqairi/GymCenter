using GymCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymCenter.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TrainerController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Home()
        {
            ViewData["TrainerUserId"] = HttpContext.Session.GetInt32("TrainerUserId");
            ViewData["TrainerFullName"] = HttpContext.Session.GetString("TrainerFullName");
            ViewData["TrainerEmail"] = HttpContext.Session.GetString("TrainerEmail");
            ViewData["TrainerImg"] = HttpContext.Session.GetString("TrainerImg");

            var membersWithDetails = await _context.Members
            .Include(m => m.User)
            .Include(m => m.Plan)
            .ToListAsync();

            return View(membersWithDetails);

        }
    }
}
