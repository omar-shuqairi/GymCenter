using GymCenter.Models;
using GymCenter.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymCenter.Controllers
{
    public class MemberController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public MemberController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

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

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult Testimonials()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult MyPlan()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}
