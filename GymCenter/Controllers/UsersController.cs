using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymCenter.Models;

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

            var members = from member in _context.Members
                          join user in _context.Users on member.Userid equals user.Userid
                          join UserLogin in _context.UserLogins on user.Userid equals UserLogin.Userid
                          //join Workoutplan in _context.Workoutplans on member.Planid equals Workoutplan.Planid
                          where UserLogin.Roleid == 3
                          select new JoinMemberUserTables
                          {
                              Fname = user.Fname,
                              Lname = user.Lname,
                              SubscriptionStart = member.SubscriptionStart,
                              SubscriptionEnd = member.SubscriptionEnd,
                              Email = user.Email,
                              ImagePath = user.ImagePath,
                              Planid = member.Planid,
                              Username = UserLogin.Username,
                              Passwordd = UserLogin.Passwordd,
                              Userid = user.Userid


                          };

            var result = members.ToList();

            return View(result);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Fname,Lname,Username,Passwordd,Email,ImageFile")] JoinAdminUserTables newuser)
        {
            if (ModelState.IsValid)
            {

                if (newuser.ImageFile != null)
                {
                    string wwwRootpath = _webHostEnvironment.WebRootPath;
                    string filename = Guid.NewGuid().ToString() + "_" + newuser.ImageFile.FileName;
                    string path = Path.Combine(wwwRootpath + "/images/", filename);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await newuser.ImageFile.CopyToAsync(fileStream);
                    }
                    newuser.ImagePath = filename;
                }

                User user = new User();
                user.Fname = newuser.Fname;
                user.Lname = newuser.Lname;
                user.Email = newuser.Email;
                user.ImagePath = newuser.ImagePath;


                _context.Add(user);
                await _context.SaveChangesAsync();

                UserLogin userLogin = new UserLogin();
                userLogin.Username = newuser.Username;
                userLogin.Roleid = 3;
                userLogin.Passwordd = newuser.Passwordd;
                userLogin.Userid = user.Userid;

                _context.Add(userLogin);
                await _context.SaveChangesAsync();

                Member member = new Member();
                member.Userid = user.Userid;
                _context.Add(member);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            return View(newuser);
        }

        // GET: Users/Edit/5
        public IActionResult Edit(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var Member = (from User in _context.Users
                          join UserLogin in _context.UserLogins
                          on User.Userid equals UserLogin.Userid
                          join findmember in _context.Members
                          on User.Userid equals findmember.Userid
                          where User.Userid == id
                          select new JoinMemberUserTables
                          {
                              Userid = User.Userid,
                              Fname = User.Fname,
                              Lname = User.Lname,
                              SubscriptionStart = findmember.SubscriptionStart,
                              SubscriptionEnd = findmember.SubscriptionEnd,
                              Email = User.Email,
                              ImagePath = User.ImagePath,
                              Planid = findmember.Planid,
                              Username = UserLogin.Username,
                              Passwordd = UserLogin.Passwordd,


                          }).FirstOrDefault();
            if (Member == null)
            {
                return NotFound();
            }
            return View(Member);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Userid,Fname,Lname,Username,Passwordd,Email,ImageFile")] JoinMemberUserTables member)

        {
            if (id != member.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (member.ImageFile != null)
                    {
                        string wwwRootpath = _webHostEnvironment.WebRootPath;
                        string filename = Guid.NewGuid().ToString() + "_" + member.ImageFile.FileName;
                        string path = Path.Combine(wwwRootpath + "/images/", filename);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await member.ImageFile.CopyToAsync(fileStream);
                        }
                        member.ImagePath = filename;
                    }

                    var user = await _context.Users.FindAsync(id);
                    user.Fname = member.Fname;
                    user.Email = member.Email;
                    user.Lname = member.Lname;
                    user.ImagePath = member.ImagePath;


                    _context.Update(user);
                    await _context.SaveChangesAsync();


                    var userlogin = await _context.UserLogins
                               .Where(ul => ul.Userid == user.Userid)
                               .FirstOrDefaultAsync();
                    userlogin.Username = member.Username;
                    userlogin.Passwordd = member.Passwordd;
                    _context.Update(userlogin);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(member.Userid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Users/Delete/5
        public IActionResult Delete(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }
            var Member = (from User in _context.Users
                          join UserLogin in _context.UserLogins
                          on User.Userid equals UserLogin.Userid
                          join findmember in _context.Members
                          on User.Userid equals findmember.Userid
                          where User.Userid == id
                          select new JoinMemberUserTables
                          {
                              Userid = User.Userid,
                              Fname = User.Fname,
                              Lname = User.Lname,
                              SubscriptionStart = findmember.SubscriptionStart,
                              SubscriptionEnd = findmember.SubscriptionEnd,
                              Email = User.Email,
                              ImagePath = User.ImagePath,
                              Planid = findmember.Planid,
                              Username = UserLogin.Username,
                              Passwordd = UserLogin.Passwordd
                          }).FirstOrDefault();
            if (Member == null)
            {
                return NotFound();
            }
            return View(Member);
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
            var user = await _context.Users.FindAsync(id);
            var userlogin = await _context.UserLogins
                              .Where(ul => ul.Userid == user.Userid)
                              .FirstOrDefaultAsync();
            var member = await _context.Members
                              .Where(ul => ul.Userid == user.Userid)
                              .FirstOrDefaultAsync();
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            if (userlogin != null)
            {
                _context.UserLogins.Remove(userlogin);
            }
            if (member != null)
            {
                _context.Members.Remove(member);
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
