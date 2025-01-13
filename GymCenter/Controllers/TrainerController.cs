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


        public IActionResult Profile()
        {
            int? AdminId = HttpContext.Session.GetInt32("TrainerUserId");
            ViewData["TrainerFullName"] = HttpContext.Session.GetString("TrainerFullName");
            ViewData["TrainerEmail"] = HttpContext.Session.GetString("TrainerEmail");
            ViewData["TrainerImg"] = HttpContext.Session.GetString("TrainerImg");
            var trainerDetails = (from User in _context.Users
                                  join UserLogin in _context.UserLogins
                                  on User.Userid equals UserLogin.Userid
                                  where User.Userid == AdminId
                                  select new JoinTrainerUserTables
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

            if (trainerDetails == null)
            {
                return NotFound();
            }
            return View(trainerDetails);

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

            HttpContext.Session.SetString("TrainerFullName", user.Fname + " " + user.Lname);
            HttpContext.Session.SetString("TrainerEmail", user.Email);
            HttpContext.Session.SetString("TrainerImg", user.ImagePath);

            return RedirectToAction(nameof(Profile));

        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Home", "Guest");
        }
    }
}
