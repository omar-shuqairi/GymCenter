using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymCenter.Models;
using GymCenter.Enums;
using SQLitePCL;

namespace GymCenter.Controllers
{
    public class UsersController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            var MembersList = await _context.Members
            .Include(m => m.User)
            .ThenInclude(u => u.UserLogins)
            .Include(m => m.Plan)
            .ToListAsync();
            return View(MembersList);

        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var MemberDetails = await _context.Members
            .Include(m => m.User)
            .ThenInclude(u => u.UserLogins)
            .Include(m => m.Plan)
            .SingleOrDefaultAsync(m => m.Userid == id);

            if (MemberDetails == null)
            {
                return NotFound();
            }
            return View(MemberDetails);
        }


        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Fname,Lname,Email,ImagePath,ImageFile")] User NewMember, string Username, string Passwordd)
        {
            if (ModelState.IsValid)
            {
                if (NewMember.ImageFile != null)
                {
                    string wwwRootpath = _webHostEnvironment.WebRootPath;
                    string filename = Guid.NewGuid().ToString() + "_" + NewMember.ImageFile.FileName;
                    string path = Path.Combine(wwwRootpath + "/images/", filename);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await NewMember.ImageFile.CopyToAsync(fileStream);
                    }
                    NewMember.ImagePath = filename;
                }

                _context.Add(NewMember);
                await _context.SaveChangesAsync();

                UserLogin userLogin = new UserLogin();
                userLogin.Username = Username;
                userLogin.Roleid = (decimal?)EnumRole.Member;
                userLogin.Passwordd = Passwordd;
                userLogin.Userid = NewMember.Userid;

                _context.Add(userLogin);
                await _context.SaveChangesAsync();

                Member member = new Member();
                member.Userid = NewMember.Userid;
                _context.Add(member);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            return View(NewMember);
        }


        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var MemberDetails = await _context.Members
            .Include(m => m.User)
            .ThenInclude(u => u.UserLogins)
            .Include(m => m.Plan)
            .SingleOrDefaultAsync(m => m.Userid == id);

            if (MemberDetails == null)
            {
                return NotFound();
            }
            return View(MemberDetails);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(decimal id, [Bind("Userid,Fname,Lname,Email,ImageFile")] User MemberNewDetails, string Username, string NewPassword, string ConfirmPassword)
        {

            if (id != MemberNewDetails.Userid)
            {
                return NotFound();
            }
            var MemberSavedDetails = await _context.Users
                .Include(u => u.UserLogins)
                .SingleOrDefaultAsync(u => u.Userid == MemberNewDetails.Userid);

            if (MemberSavedDetails == null)
            {
                return NotFound("User not found");
            }

            var MemberSavedUserLogin = MemberSavedDetails.UserLogins.FirstOrDefault();
            if (MemberSavedUserLogin == null)
            {
                return NotFound("User login details not found");
            }

            if (MemberNewDetails.ImageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString() + "_" + MemberNewDetails.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/images/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await MemberNewDetails.ImageFile.CopyToAsync(fileStream);
                }
                MemberSavedDetails.ImagePath = filename;
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

            MemberSavedDetails.Fname = MemberNewDetails.Fname;
            MemberSavedDetails.Lname = MemberNewDetails.Lname;
            MemberSavedDetails.Email = MemberNewDetails.Email;
            MemberSavedUserLogin.Username = Username;

            _context.Update(MemberSavedDetails);
            await _context.SaveChangesAsync();

            TempData["UpdateMemberDetailes"] = "Your changes has been updated successfully!";
            return RedirectToAction(nameof(Edit));
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var MemberDetails = await _context.Members
            .Include(m => m.User)
            .ThenInclude(u => u.UserLogins)
            .Include(m => m.Plan)
            .SingleOrDefaultAsync(m => m.Userid == id);

            if (MemberDetails == null)
            {
                return NotFound();
            }
            return View(MemberDetails);
        }









        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ModelContext.Users'  is null.");
            }
            var MemberDetails = await _context.Members
            .Include(m => m.User)
            .ThenInclude(u => u.UserLogins)
            .Include(m => m.Plan)
            .SingleOrDefaultAsync(m => m.Userid == id);
            if (MemberDetails != null)
            {
                _context.UserLogins.Remove(MemberDetails.User.UserLogins.SingleOrDefault(x => x.Userid == MemberDetails.Userid));
                _context.Members.Remove(MemberDetails);
                _context.Users.Remove(MemberDetails.User);
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
