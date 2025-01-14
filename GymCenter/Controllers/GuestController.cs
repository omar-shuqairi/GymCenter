﻿using GymCenter.Models;
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
                return RedirectToAction(nameof(Contact));
            }
            return View(contactform);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
