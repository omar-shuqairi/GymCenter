using GymCenter.Enums;
using GymCenter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymCenter.Controllers
{
    public class ManageTrainersController : Controller
    {


        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ManageTrainersController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            var TrainersList = await _context.UserLogins
            .Include(t => t.User)
            .Where(t => t.Roleid == (decimal?)EnumRole.Trainer)
            .ToListAsync();
            return View(TrainersList);
        }

        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var TrainerDetails = await _context.UserLogins
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.Userid == id);
            if (TrainerDetails == null)
            {
                return NotFound();
            }
            return View(TrainerDetails);
        }

        public IActionResult Create()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Fname,Lname,Email,ImagePath,ImageFile")] User NewTrainer, string Username, string Passwordd)
        {
            if (ModelState.IsValid)
            {
                if (NewTrainer.ImageFile != null)
                {
                    string wwwRootpath = _webHostEnvironment.WebRootPath;
                    string filename = Guid.NewGuid().ToString() + "_" + NewTrainer.ImageFile.FileName;
                    string path = Path.Combine(wwwRootpath + "/images/", filename);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await NewTrainer.ImageFile.CopyToAsync(fileStream);
                    }
                    NewTrainer.ImagePath = filename;
                }

                _context.Add(NewTrainer);
                await _context.SaveChangesAsync();

                UserLogin userLogin = new UserLogin();
                userLogin.Username = Username;
                userLogin.Roleid = (decimal?)EnumRole.Trainer;
                userLogin.Passwordd = Passwordd;
                userLogin.Userid = NewTrainer.Userid;

                _context.Add(userLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(NewTrainer);
        }

        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var TrainerDetails = await _context.UserLogins
            .Include(t => t.User)
            .SingleOrDefaultAsync(m => m.Userid == id);

            if (TrainerDetails == null)
            {
                return NotFound();
            }
            return View(TrainerDetails);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(decimal id, [Bind("Userid,Fname,Lname,Email,ImageFile")] User TrainerNewDetails, string Username, string NewPassword, string ConfirmPassword)
        {

            if (id != TrainerNewDetails.Userid)
            {
                return NotFound();
            }
            var TrainerSavedDetails = await _context.Users
                .Include(u => u.UserLogins)
                .SingleOrDefaultAsync(u => u.Userid == TrainerNewDetails.Userid);

            if (TrainerSavedDetails == null)
            {
                return NotFound("User not found");
            }

            var MemberSavedUserLogin = TrainerSavedDetails.UserLogins.FirstOrDefault();
            if (MemberSavedUserLogin == null)
            {
                return NotFound("User login details not found");
            }

            if (TrainerNewDetails.ImageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString() + "_" + TrainerNewDetails.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/images/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await TrainerNewDetails.ImageFile.CopyToAsync(fileStream);
                }
                TrainerSavedDetails.ImagePath = filename;
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
                    return RedirectToAction(nameof(Edit));
                }
            }

            TrainerSavedDetails.Fname = TrainerNewDetails.Fname;
            TrainerSavedDetails.Lname = TrainerNewDetails.Lname;
            TrainerSavedDetails.Email = TrainerNewDetails.Email;
            MemberSavedUserLogin.Username = Username;
            _context.Update(TrainerSavedDetails);
            await _context.SaveChangesAsync();
            TempData["UpdateTrainerDetailes"] = "Your changes has been updated successfully!";
            return RedirectToAction(nameof(Edit));
        }

        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var TrainerDetails = await _context.UserLogins
            .Include(t => t.User)
            .SingleOrDefaultAsync(t => t.Userid == id);
            if (TrainerDetails == null)

                if (TrainerDetails == null)
                {
                    return NotFound();
                }
            return View(TrainerDetails);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ModelContext.Users'  is null.");
            }
            var TrainerDetails = await _context.Users
            .Include(m => m.UserLogins)
            .SingleOrDefaultAsync(m => m.Userid == id);
            if (TrainerDetails != null)
            {
                _context.UserLogins.Remove(TrainerDetails.UserLogins.SingleOrDefault(x => x.Userid == TrainerDetails.Userid));
                _context.Users.Remove(TrainerDetails);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(decimal id)
        {
            return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
        }

    }
}
