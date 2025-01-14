using GymCenter.Models;
using GymCenter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace GymCenter.Controllers
{
	public class MemberController : Controller
	{
		private readonly ModelContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly EmailService _emailService;


		public MemberController(ModelContext context, IWebHostEnvironment webHostEnvironment, EmailService emailService)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
			_emailService = emailService;
		}
		public async Task<IActionResult> Home()
		{
			var homepageList = await _context.Homepages.ToListAsync();
			return View(homepageList);

		}

		public async Task<IActionResult> About()
		{

			var aboutus = await _context.Aboutuspages.FirstOrDefaultAsync();
			var choose = await _context.Whychooseus.ToListAsync();
			var shredimg = await _context.Siteinfos
			.Select(s => s.SharedImagePath)
			.FirstOrDefaultAsync();

			var model = Tuple.Create(aboutus, choose, shredimg);


			return View(model);
		}

		//here edit this to subscribe and form from memberdesgin 
		public async Task<IActionResult> Services()
		{

			var workoutplans = await _context.Workoutplans.ToListAsync();
			var shredimg = await _context.Siteinfos
			.Select(s => s.SharedImagePath)
			.FirstOrDefaultAsync();
			var model = Tuple.Create(workoutplans, shredimg);
			return View(model);
		}



		public async Task<IActionResult> Testimonials()
		{
			var testimonials = await _context.Testimonials
			.Include(t => t.User)
			.Where(t => t.Status == "Approved")
			.ToListAsync();
			var shredimg = await _context.Siteinfos
			.Select(s => s.SharedImagePath)
			.FirstOrDefaultAsync();

			var model = Tuple.Create(testimonials, shredimg);
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Testimonials(string content)
		{
			if (ModelState.IsValid)
			{
				int? memberuserid = HttpContext.Session.GetInt32("MemberuserId");
				var testimonial = new Testimonial();
				testimonial.Userid = memberuserid;
				testimonial.Content = content;
				testimonial.Status = "Pending";
				_context.Add(testimonial);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Testimonials));
			}
			return View(content);
		}

		public async Task<IActionResult> Contact()
		{
			var shredimg = await _context.Siteinfos
			.Select(s => s.SharedImagePath)
			.FirstOrDefaultAsync();

			var contactinfo = await _context.Contactus.FirstOrDefaultAsync();

			var model = Tuple.Create(contactinfo, shredimg);

			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Contact([Bind("Guestname,Guestemail,Guestcomment")] Contactform contactform)
		{
			if (ModelState.IsValid)
			{
				_context.Add(contactform);
				await _context.SaveChangesAsync();

				// Compose an email
				string subject = "Thank you for contacting us!";
				string body = $@"
                 <p>Dear {contactform.Guestname},</p>
                 <p>Thank you for reaching out to us. We have received your message:</p>
                 <blockquote>{contactform.Guestcomment}</blockquote>
                 <p>Our team will get back to you as soon as possible.</p>
                 <p>Best regards,</p>
                 <p>Your GymCenter Team</p>";
				await _emailService.SendEmailAsync(contactform.Guestemail, subject, body);
				return RedirectToAction(nameof(Contact));
			}
			return View(contactform);
		}




		public async Task<IActionResult> Profile()
		{
			int? memberuserid = HttpContext.Session.GetInt32("MemberuserId");
			ViewData["MemberImg"] = HttpContext.Session.GetString("MemberImg");
			var shredimg = await _context.Siteinfos
		   .Select(s => s.SharedImagePath)
		   .FirstOrDefaultAsync();
			ViewBag.SharedImage = shredimg;
			var memberDetails = await (from User in _context.Users
									   join UserLogin in _context.UserLogins
									   on User.Userid equals UserLogin.Userid
									   where User.Userid == memberuserid
									   select new JoinMemberUserTables
									   {
										   Userid = User.Userid,
										   Fname = User.Fname,
										   Lname = User.Lname,
										   Username = UserLogin.Username,
										   Email = User.Email,
										   ImagePath = User.ImagePath,
										   Passwordd = UserLogin.Passwordd,
										   ImageFile = User.ImageFile
									   }).FirstOrDefaultAsync();

			if (memberDetails == null)
			{
				return NotFound();
			}

			return View(memberDetails);
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

			HttpContext.Session.SetString("MemberImg", user.ImagePath);

			return RedirectToAction(nameof(Profile));

		}



















		public IActionResult MyPlan()
		{
			return View();
		}
















		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Home", "Guest");
		}
	}
}
