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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([Bind("Username,Passwordd")] UserLogin userLogin)
        {
            var auth = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).SingleOrDefault();
            var EmailUser = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.Email).SingleOrDefault();
            var Fname = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.Fname).SingleOrDefault();
            var Lname = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.Lname).SingleOrDefault();
            var imgpath = _context.UserLogins.Where(x => x.Username == userLogin.Username && x.Passwordd == userLogin.Passwordd).Select(x => x.User.ImagePath).SingleOrDefault();
            if (auth != null)
            {
                switch (auth.Roleid)
                {
                    case 1://Admin
                        HttpContext.Session.SetInt32("AdminUserId", (int)auth.Userid);
                        HttpContext.Session.SetString("AdminFullName", Fname + " " + Lname);
                        HttpContext.Session.SetString("AdminEmail", EmailUser);
                        HttpContext.Session.SetString("AdminImg", imgpath);

                        return RedirectToAction("Home", "Admin");

                    case 2://Trainer 
                        HttpContext.Session.SetString("TrainerFullName", Fname + " " + Lname);
                        HttpContext.Session.SetString("TrainerEmail", EmailUser);
                        return RedirectToAction("Home", "Trainer");

                    case 3://Member 
                        HttpContext.Session.SetInt32("userId", (int)auth.Userid);
                        HttpContext.Session.SetString("MemberEmail", EmailUser);
                        return RedirectToAction("Home", "Member");
                }

            }

            return View();
        }

        public IActionResult SignUp()
        {
            return View();
        }
    }
}
