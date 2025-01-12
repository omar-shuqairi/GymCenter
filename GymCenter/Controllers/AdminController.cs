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


        public IActionResult Search()
        {
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");

            var memberDetails = from member in _context.Members
                                join user in _context.Users on member.Userid equals user.Userid
                                select new JoinMemberUserTables
                                {
                                    Fname = user.Fname,
                                    Lname = user.Lname,
                                    SubscriptionStart = member.SubscriptionStart,
                                    SubscriptionEnd = member.SubscriptionEnd
                                };

            var result = memberDetails.ToList();

            return View(result);

        }
        [HttpPost]
        public IActionResult Search(DateTime? startDate, DateTime? endDate)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            var memberDetails = from member in _context.Members
                                join user in _context.Users on member.Userid equals user.Userid
                                select new JoinMemberUserTables
                                {
                                    Fname = user.Fname,
                                    Lname = user.Lname,
                                    SubscriptionStart = member.SubscriptionStart,
                                    SubscriptionEnd = member.SubscriptionEnd
                                };

            var result = memberDetails.ToList();
            if (startDate == null && endDate == null)
            {
                ViewBag.TotalSubscriptions = result.Where(m => m.SubscriptionStart != null && m.SubscriptionEnd != null).Count();
                return View(result);
            }
            else if (startDate != null && endDate == null)
            {
                result = result.Where(x => x.SubscriptionStart.HasValue && x.SubscriptionStart.Value.Date >= startDate).ToList();
                ViewBag.TotalSubscriptions = result.Where(m => m.SubscriptionStart != null && m.SubscriptionEnd != null).Count();
                return View(result);
            }
            else if (startDate == null && endDate != null)
            {


                result = result.Where(x => x.SubscriptionStart.HasValue && x.SubscriptionStart.Value.Date <= endDate).ToList();
                ViewBag.TotalSubscriptions = result.Where(m => m.SubscriptionStart != null && m.SubscriptionEnd != null).Count();
                return View(result);
            }

            else
            {
                result = result.Where(x => x.SubscriptionStart.HasValue && x.SubscriptionStart.Value.Date >= startDate && x.SubscriptionEnd.HasValue && x.SubscriptionEnd.Value.Date <= endDate).ToList();
                ViewBag.TotalSubscriptions = result.Where(m => m.SubscriptionStart != null && m.SubscriptionEnd != null).Count();

            }
            return View(result);

        }

        public IActionResult Profile()
        {
            ViewData["AdmimUserId"] = HttpContext.Session.GetInt32("AdminUserId");
            int? AdminId = HttpContext.Session.GetInt32("AdminUserId");
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");


            var adminDetails = (from User in _context.Users
                                join UserLogin in _context.UserLogins
                                on User.Userid equals UserLogin.Userid
                                where User.Userid == AdminId
                                select new JoinAdminUserTables
                                {
                                    Userid = User.Userid,
                                    Fname = User.Fname,
                                    Lname = User.Lname,
                                    Username = UserLogin.Username,
                                    Email = User.Email,
                                    ImagePath = User.ImagePath,
                                    Passwordd = UserLogin.Passwordd,
                                    ImageFile = User.ImageFile

                                }).FirstOrDefault();

            if (adminDetails == null)
            {
                return NotFound();
            }
            return View(adminDetails);

        }
        [HttpPost]
        public async Task<IActionResult> Profile([Bind("Userid,Fname,Lname,Email,ImageFile")] User user, string Username, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            if (user.ImageFile != null)
            {
                string wwwRootpath = _webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString() + "_" + user.ImageFile.FileName;
                string path = Path.Combine(wwwRootpath + "/images/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await user.ImageFile.CopyToAsync(fileStream);
                }
                user.ImagePath = filename;
            }
            _context.Update(user);
            await _context.SaveChangesAsync();

            var userlogin = await _context.UserLogins.FindAsync(user.Userid);
            userlogin.Username = Username;
            userlogin.Passwordd = NewPassword;
            _context.Update(userlogin);
            await _context.SaveChangesAsync();

            HttpContext.Session.SetString("AdminFullName", user.Fname + " " + user.Lname);
            HttpContext.Session.SetString("AdminEmail", user.Email);
            HttpContext.Session.SetString("AdminImg", user.ImagePath);

            return RedirectToAction(nameof(Profile));

        }



    }
}
