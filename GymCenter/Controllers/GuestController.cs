using GymCenter.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GymCenter.Controllers
{
    public class GuestController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<GuestController> _logger;

        public GuestController(ILogger<GuestController> logger, ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
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

        public async Task<IActionResult> Services()
        {

            var workoutplans = await _context.Workoutplans.ToListAsync();
            var shredimg = await _context.Siteinfos
            .Select(s => s.SharedImagePath)
            .FirstOrDefaultAsync();
            var model = Tuple.Create(workoutplans, shredimg);
            return View(model);
        }

        public IActionResult Testimonials()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
