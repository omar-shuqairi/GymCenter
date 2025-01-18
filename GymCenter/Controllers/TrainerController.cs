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
            .AsNoTracking()
            .ToListAsync();

            return View(membersWithDetails);

        }


        public async Task<IActionResult> Profile()
        {
            ViewData["TrainerFullName"] = HttpContext.Session.GetString("TrainerFullName");
            ViewData["TrainerEmail"] = HttpContext.Session.GetString("TrainerEmail");
            ViewData["TrainerImg"] = HttpContext.Session.GetString("TrainerImg");
            var TrainerDetails = await _context.Users
           .Include(t => t.UserLogins)
           .Where(t => t.Userid == HttpContext.Session.GetInt32("TrainerUserId"))
           .AsNoTracking()
           .FirstOrDefaultAsync();
            if (TrainerDetails == null)
            {
                return NotFound();
            }
            return View(TrainerDetails);

        }
        [HttpPost]
        public async Task<IActionResult> Profile([Bind("Userid,Fname,Lname,Email,ImageFile")] User TrainerNewProfileDetails, string Username, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var TrainerSavedProfileDetails = await _context.Users
               .Include(u => u.UserLogins)
               .SingleOrDefaultAsync(u => u.Userid == TrainerNewProfileDetails.Userid);

            if (TrainerSavedProfileDetails == null)
            {
                return NotFound("User not found");
            }

            var MemberSavedUserLogin = TrainerSavedProfileDetails.UserLogins.FirstOrDefault();
            if (MemberSavedUserLogin == null)
            {
                return NotFound("User login details not found");
            }

            if (!string.IsNullOrEmpty(CurrentPassword) && MemberSavedUserLogin.Passwordd != CurrentPassword)
            {
                TempData["ErrorPass"] = "Your password is incorrect!";
                return RedirectToAction(nameof(Profile));
            }

            if (TrainerNewProfileDetails.ImageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString() + "_" + TrainerNewProfileDetails.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/images/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await TrainerNewProfileDetails.ImageFile.CopyToAsync(fileStream);
                }
                TrainerSavedProfileDetails.ImagePath = filename;
            }

            if (!string.IsNullOrEmpty(NewPassword))
            {
                if (NewPassword == ConfirmPassword)
                {
                    MemberSavedUserLogin.Passwordd = NewPassword;
                }
                else
                {
                    TempData["ErrorPassMatch"] = "The new password and confirmation password do not match!";
                    return RedirectToAction(nameof(Profile));
                }
            }

            TrainerSavedProfileDetails.Fname = TrainerNewProfileDetails.Fname;
            TrainerSavedProfileDetails.Lname = TrainerNewProfileDetails.Lname;
            TrainerSavedProfileDetails.Email = TrainerNewProfileDetails.Email;
            MemberSavedUserLogin.Username = Username;

            _context.Update(TrainerSavedProfileDetails);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(TrainerSavedProfileDetails.ImagePath))
            {
                HttpContext.Session.SetString("TrainerImg", TrainerSavedProfileDetails.ImagePath);
            }
            if (!string.IsNullOrEmpty(TrainerSavedProfileDetails.Fname) || !string.IsNullOrEmpty(TrainerSavedProfileDetails.Lname))
            {
                HttpContext.Session.SetString("TrainerFullName", TrainerSavedProfileDetails.Fname + " " + TrainerSavedProfileDetails.Lname);
            }
            if (!string.IsNullOrEmpty(TrainerSavedProfileDetails.Email))
            {
                HttpContext.Session.SetString("TrainerEmail", TrainerSavedProfileDetails.Email);
            }
            TempData["UpdateProfile"] = "Your profile has been updated successfully!";
            return RedirectToAction(nameof(Profile));

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Home", "Guest");
        }
    }
}
