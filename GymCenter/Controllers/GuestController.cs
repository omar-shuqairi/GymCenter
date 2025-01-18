using GymCenter.Models;
using GymCenter.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
namespace GymCenter.Controllers
{
    public class GuestController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<GuestController> _logger;
        private readonly EmailService _emailService;

        public GuestController(ILogger<GuestController> logger, ModelContext context, IWebHostEnvironment webHostEnvironment, EmailService emailService)
        {
            _logger = logger;
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
                TempData["SubmissionSuccess"] = true;
                return RedirectToAction(nameof(Contact));
            }
            return View(contactform);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetSharedImage() => await _context.Siteinfos
                .Select(s => s.SharedImagePath)
                .AsNoTracking()
                .FirstOrDefaultAsync();
    }
}
