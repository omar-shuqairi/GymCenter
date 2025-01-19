using GymCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymCenter.Controllers
{
    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Home()
        {

            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            ViewData["TotalMembers"] = _context.Members.Count();
            ViewData["TotalRevenue"] = _context.Payments.Sum(p => p.Amountpaid);
            ViewData["TotalActiveSubscriptions"] = _context.Members.Where(m => m.SubscriptionEnd > DateTime.Now).Count();
            ViewData["TotalSubscriptions"] = _context.Members.Where(m => m.SubscriptionStart != null && m.SubscriptionEnd != null).Count();
            var monthlyReport = await GetMonthlyReport();
            var annualReport = await GetAnnualReport();

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
                .Where(m => m.SubscriptionStart != null)
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
                .Where(m => m.SubscriptionStart != null)
                .GroupBy(m => m.SubscriptionStart.Value.Year)
                .Select(g => new SubscriptionReport
                {
                    Year = g.Key,
                    SubscriptionsCount = g.Count()
                })
                .OrderBy(r => r.Year)
                .ToListAsync();
        }


        public async Task<IActionResult> Search()
        {

            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            var result = await _context.Members
            .Include(r => r.User)
            .Where(r => r.SubscriptionStart != null && r.SubscriptionEnd != null)
            .AsNoTracking()
            .ToListAsync();
            return View(result);

        }

        [HttpPost]
        public async Task<IActionResult> Search(DateTime? startDate, DateTime? endDate)
        {

            var result = await _context.Members
                .Include(r => r.User)
                .Where(r => r.SubscriptionStart != null && r.SubscriptionEnd != null)
                .ToListAsync();
            if (startDate == null && endDate == null)
            {
                ViewBag.TotalSubscriptions = result.Count();
                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                result = result.Where(x => x.SubscriptionStart.HasValue && x.SubscriptionStart.Value.Date >= startDate).ToList();
                ViewBag.TotalSubscriptions = result.Count();
                return View(result);
            }
            else if (startDate == null && endDate != null)
            {


                result = result.Where(x => x.SubscriptionStart.HasValue && x.SubscriptionStart.Value.Date <= endDate).ToList();
                ViewBag.TotalSubscriptions = result.Count();
                return View(result);
            }
            else
            {
                result = result.Where(x => x.SubscriptionStart.HasValue && x.SubscriptionStart.Value.Date >= startDate && x.SubscriptionEnd.HasValue && x.SubscriptionEnd.Value.Date <= endDate).ToList();
                ViewBag.TotalSubscriptions = result.Count();

            }
            return View(result);

        }

        public async Task<IActionResult> Profile()
        {

            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");

            var AdminDetails = await _context.Users
            .Include(t => t.UserLogins)
            .Where(t => t.Userid == HttpContext.Session.GetInt32("AdminUserId"))
            .AsNoTracking()
            .FirstOrDefaultAsync();
            if (AdminDetails == null)
            {
                return NotFound();
            }
            return View(AdminDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Profile([Bind("Userid,Fname,Lname,Email,ImageFile")] User AdminNewProfileDetails, string Username, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var AdminSavedProfileDetails = await _context.Users
               .Include(u => u.UserLogins)
               .SingleOrDefaultAsync(u => u.Userid == AdminNewProfileDetails.Userid);

            if (AdminSavedProfileDetails == null)
            {
                return NotFound("User not found");
            }

            var AdminSavedUserLogin = AdminSavedProfileDetails.UserLogins.FirstOrDefault();
            if (AdminSavedUserLogin == null)
            {
                return NotFound("User login details not found");
            }

            if (!string.IsNullOrEmpty(CurrentPassword) && AdminSavedUserLogin.Passwordd != CurrentPassword)
            {
                TempData["ErrorPass"] = "Your password is incorrect!";
                return RedirectToAction(nameof(Profile));
            }

            if (AdminNewProfileDetails.ImageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString() + "_" + AdminNewProfileDetails.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/images/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await AdminNewProfileDetails.ImageFile.CopyToAsync(fileStream);
                }
                AdminSavedProfileDetails.ImagePath = filename;
            }

            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (NewPassword == ConfirmPassword)
                {
                    AdminSavedUserLogin.Passwordd = NewPassword;
                }
                else
                {
                    TempData["ErrorPassMatch"] = "The new password and confirmation password do not match!";
                    return RedirectToAction(nameof(Profile));
                }
            }

            AdminSavedProfileDetails.Fname = AdminNewProfileDetails.Fname;
            AdminSavedProfileDetails.Lname = AdminNewProfileDetails.Lname;
            AdminSavedProfileDetails.Email = AdminNewProfileDetails.Email;
            AdminSavedUserLogin.Username = Username;

            _context.Update(AdminSavedProfileDetails);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(AdminSavedProfileDetails.ImagePath))
            {
                HttpContext.Session.SetString("AdminImg", AdminSavedProfileDetails.ImagePath);
            }
            if (!string.IsNullOrEmpty(AdminSavedProfileDetails.Fname) || !string.IsNullOrEmpty(AdminSavedProfileDetails.Lname))
            {
                HttpContext.Session.SetString("AdminFullName", AdminSavedProfileDetails.Fname + " " + AdminSavedProfileDetails.Lname);
            }
            if (!string.IsNullOrEmpty(AdminSavedProfileDetails.Email))
            {
                HttpContext.Session.SetString("AdminEmail", AdminSavedProfileDetails.Email);
            }
            TempData["UpdateProfile"] = "Your profile has been updated successfully!";
            return RedirectToAction(nameof(Profile));

        }


        public async Task<IActionResult> ApprovedTestimonials()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            var ApprovedTestimonials = await _context.Testimonials
                          .Where(t => t.Status == "Approved")
                          .Include(t => t.User)
                          .AsNoTracking()
                          .ToListAsync();

            return View(ApprovedTestimonials);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApprovedTestimonials(decimal? id)
        {
            var Testimonial = await _context.Testimonials.FindAsync(id);
            if (Testimonial != null)
            {
                Testimonial.Status = "Approved";
                _context.Update(Testimonial);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(PendingTestimonials));
        }

        public async Task<IActionResult> RejectedTestimonials()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");

            var RejectedTestimonials = await _context.Testimonials
                          .Where(t => t.Status == "Rejected")
                          .Include(t => t.User)
                          .AsNoTracking()
                          .ToListAsync();
            return View(RejectedTestimonials);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectedTestimonials(decimal? id)
        {
            var Testimonial = await _context.Testimonials.FindAsync(id);
            if (Testimonial != null)
            {
                Testimonial.Status = "Rejected";
                _context.Update(Testimonial);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(PendingTestimonials));
        }

        public async Task<IActionResult> PendingTestimonials()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            var PendingTestimonials = await _context.Testimonials
                          .Where(t => t.Status == "Pending")
                          .Include(t => t.User)
                          .AsNoTracking()
                          .ToListAsync();

            return View(PendingTestimonials);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Home", "Guest");
        }
    }
}