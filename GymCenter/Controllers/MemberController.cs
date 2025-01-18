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

            var HomepageContent = await _context.Homepages.AsNoTracking().ToListAsync();
            return View(HomepageContent);

        }

        public async Task<IActionResult> About()
        {

            var AboutUsContnet = await _context.Aboutuspages.AsNoTracking().FirstOrDefaultAsync();
            var WhyChoosUsContnet = await _context.Whychooseus.AsNoTracking().ToListAsync();
            var SharedImg = await GetSharedImage();
            var model = Tuple.Create(AboutUsContnet, WhyChoosUsContnet, SharedImg);
            return View(model);
        }

        public async Task<IActionResult> Services()
        {
            var WorkoutPlansContent = await _context.Workoutplans.AsNoTracking().ToListAsync();
            var SharedImg = await GetSharedImage();
            var model = Tuple.Create(WorkoutPlansContent, SharedImg);
            return View(model);
        }

        [HttpPost]
        public IActionResult Services(int Planid)
        {
            return RedirectToAction("Index", "ProceedPayment", new { PlanId = Planid });
        }

        public async Task<IActionResult> Testimonials()
        {
            var TestimonialsContent = await _context.Testimonials
            .Include(t => t.User)
            .Where(t => t.Status == "Approved")
            .AsNoTracking()
            .ToListAsync();
            var SharedImg = await GetSharedImage();
            var model = Tuple.Create(TestimonialsContent, SharedImg);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Testimonials(string NewTestimonialContent)
        {
            if (ModelState.IsValid)
            {
                var NewTestimonial = new Testimonial();
                NewTestimonial.Userid = HttpContext.Session.GetInt32("MemberuserId");
                NewTestimonial.Content = NewTestimonialContent;
                NewTestimonial.Status = "Pending";
                _context.Add(NewTestimonial);
                await _context.SaveChangesAsync();
                TempData["SubmitTestimonial"] = "Thank you for submitting your testimonial!";
                return RedirectToAction(nameof(Testimonials));
            }
            return View(NewTestimonialContent);
        }

        public async Task<IActionResult> Contact()
        {
            var ContactInfoContent = await _context.Contactus.AsNoTracking().FirstOrDefaultAsync();
            var SharedImg = await GetSharedImage();
            var model = Tuple.Create(ContactInfoContent, SharedImg);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Contact([Bind("Guestname,Guestemail,Guestcomment")] Contactform contactform)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactform);
                await _context.SaveChangesAsync();

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
            ViewData["MemberImg"] = HttpContext.Session.GetString("MemberImg");
            var SharedImg = await GetSharedImage();
            var MmemberDetails = await _context.Users
            .Include(m => m.UserLogins)
            .Include(m => m.Invoices)
            .Where(m => m.Userid == HttpContext.Session.GetInt32("MemberuserId"))
            .AsNoTracking()
            .FirstOrDefaultAsync();

            if (MmemberDetails == null)
            {
                return NotFound();
            }
            var model = Tuple.Create(MmemberDetails, SharedImg);
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Profile([Bind("Userid,Fname,Lname,Email,ImageFile")] User MemberNewProfileDetails, string Username, string CurrentPassword, string NewPassword, string ConfirmPassword)
        {
            var MemberSavedProfileDetails = await _context.Users
                .Include(u => u.UserLogins)
                .SingleOrDefaultAsync(u => u.Userid == MemberNewProfileDetails.Userid);

            if (MemberSavedProfileDetails == null)
            {
                return NotFound("User not found");
            }

            var MemberSavedUserLogin = MemberSavedProfileDetails.UserLogins.FirstOrDefault();
            if (MemberSavedUserLogin == null)
            {
                return NotFound("User login details not found");
            }

            if (!string.IsNullOrEmpty(CurrentPassword) && MemberSavedUserLogin.Passwordd != CurrentPassword)
            {
                TempData["ErrorPass"] = "Your current password is incorrect!";
                return RedirectToAction(nameof(Profile));
            }

            if (MemberNewProfileDetails.ImageFile != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string filename = Guid.NewGuid().ToString() + "_" + MemberNewProfileDetails.ImageFile.FileName;
                string path = Path.Combine(wwwRootPath + "/images/", filename);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await MemberNewProfileDetails.ImageFile.CopyToAsync(fileStream);
                }
                MemberSavedProfileDetails.ImagePath = filename;
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

            MemberSavedProfileDetails.Fname = MemberNewProfileDetails.Fname;
            MemberSavedProfileDetails.Lname = MemberNewProfileDetails.Lname;
            MemberSavedProfileDetails.Email = MemberNewProfileDetails.Email;
            MemberSavedUserLogin.Username = Username;

            _context.Update(MemberSavedProfileDetails);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(MemberSavedProfileDetails.ImagePath))
            {
                HttpContext.Session.SetString("MemberImg", MemberSavedProfileDetails.ImagePath);
            }

            TempData["UpdateProfile"] = "Your profile has been updated successfully!";
            return RedirectToAction(nameof(Profile));
        }
        public async Task<IActionResult> MyPlan()
        {
            int? memberuserid = HttpContext.Session.GetInt32("MemberuserId");
            var SharedImg = await GetSharedImage();
            var member = await _context.Members
            .Include(m => m.Plan)
            .Where(m => m.Userid == memberuserid)
            .AsNoTracking()
            .FirstOrDefaultAsync();
            var model = Tuple.Create(member, SharedImg);
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Home", "Guest");
        }

        public async Task<IActionResult> DownloadInvoice(int invoiceId)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(i => i.Invoiceid == invoiceId);
            if (invoice == null) return NotFound("Invoice not found.");
            return File(invoice.Pdfdata, "application/pdf", "Invoice.pdf");
        }

        private async Task<string> GetSharedImage() => await _context.Siteinfos
                .Select(s => s.SharedImagePath)
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }
}
