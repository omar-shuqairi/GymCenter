using GymCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        public AdminController(ModelContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Home()
        {
            ViewData["AdmimUserId"] = HttpContext.Session.GetInt32("AdminUserId");
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            ViewData["TotalMembers"] = _context.Members.Count();
            ViewData["TotalRevenue"] = _context.Payments.Sum(p => p.Amountpaid);
            ViewData["TotalActiveSubscriptions"] = _context.Members.Where(m => m.SubscriptionEnd > DateTime.Now).Count();
            ViewData["TotalSubscriptions"] = _context.Members.Where(m => m.SubscriptionStart != null && m.SubscriptionEnd != null).Count();
            var monthlyReport = await GetMonthlyReport();
            var annualReport = await GetAnnualReport();

            // Pass reports to the view
            var model = new SubscriptionReportsViewModel
            {
                MonthlyReport = monthlyReport,
                AnnualReport = annualReport
            };

            return View(model);
        }
        private async Task<List<SubscriptionReport>> GetMonthlyReport()
        {
            return await _context.Members
                .Where(m => m.SubscriptionStart != null)  // Ensure SubscriptionStart is not null
                .GroupBy(m => new { m.SubscriptionStart.Value.Month, m.SubscriptionStart.Value.Year })
                .Select(g => new SubscriptionReport
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    SubscriptionsCount = g.Count()
                })
                .OrderBy(r => r.Year).ThenBy(r => r.Month)
                .ToListAsync();
        }
        private async Task<List<SubscriptionReport>> GetAnnualReport()
        {
            return await _context.Members
                .Where(m => m.SubscriptionStart != null)  // Ensure SubscriptionStart is not null
                .GroupBy(m => m.SubscriptionStart.Value.Year)
                .Select(g => new SubscriptionReport
                {
                    Year = g.Key,
                    SubscriptionsCount = g.Count()
                })
                .OrderBy(r => r.Year)
                .ToListAsync();
        }
    }
}
