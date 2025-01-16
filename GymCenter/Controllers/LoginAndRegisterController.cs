using GymCenter.Models;
using Microsoft.AspNetCore.Mvc;

namespace GymCenter.Controllers
{
    public class LoginAndRegisterController : Controller
    {

        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public LoginAndRegisterController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp([Bind("Userid,Fname,Lname,Email,ImagePath,ImageFile")] User user, string username, string password)
        {
            if (ModelState.IsValid)
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
                _context.Add(user);
                await _context.SaveChangesAsync();
                UserLogin login = new UserLogin();
                login.Username = username;
                login.Passwordd = password;
                login.Userid = user.Userid;
                login.Roleid = 3;
                _context.Add(login);
                await _context.SaveChangesAsync();
                Member member = new Member();
                member.Userid = user.Userid;
                _context.Add(member);
                await _context.SaveChangesAsync();
                TempData["Welcome"] = "We are so excited to have you join us!";
                return RedirectToAction("Login");
            }
            return View(user);
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([Bind("Username,Passwordd")] UserLogin userLogin)
        {
            var auth = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).SingleOrDefault();
            if (auth != null)
            {
                var EmailUser = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.Email).SingleOrDefault();
                var Fname = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.Fname).SingleOrDefault();
                var Lname = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.Lname).SingleOrDefault();
                var imgpath = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.ImagePath).SingleOrDefault();

                switch (auth.Roleid)
                {
                    case 1://Admin
                        HttpContext.Session.SetInt32("AdminUserId", (int)auth.Userid);
                        HttpContext.Session.SetString("AdminFullName", Fname + " " + Lname);
                        HttpContext.Session.SetString("AdminEmail", EmailUser);
                        HttpContext.Session.SetString("AdminImg", imgpath);

                        return RedirectToAction("Home", "Admin");

                    case 2://Trainer
                        HttpContext.Session.SetInt32("TrainerUserId", (int)auth.Userid);
                        HttpContext.Session.SetString("TrainerFullName", Fname + " " + Lname);
                        HttpContext.Session.SetString("TrainerEmail", EmailUser);
                        HttpContext.Session.SetString("TrainerImg", imgpath);
                        return RedirectToAction("Home", "Trainer");

                    case 3://Member 
                        HttpContext.Session.SetInt32("MemberuserId", (int)auth.Userid);
                        HttpContext.Session.SetString("MemberEmail", EmailUser);
                        HttpContext.Session.SetString("MemberImg", imgpath);
                        return RedirectToAction("Home", "Member");
                }

            }
            TempData["ErrorMessage"] = "Invalid username or password.";
            return View();
        }
    }
}
