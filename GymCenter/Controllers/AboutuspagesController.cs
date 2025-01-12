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
    public class AboutuspagesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AboutuspagesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Aboutuspages
        public async Task<IActionResult> Index()
        {
            return _context.Aboutuspages != null ?
                        View(await _context.Aboutuspages.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Aboutuspages'  is null.");
        }

        // GET: Aboutuspages/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Aboutuspages == null)
            {
                return NotFound();
            }

            var aboutuspage = await _context.Aboutuspages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutuspage == null)
            {
                return NotFound();
            }

            return View(aboutuspage);
        }

        // GET: Aboutuspages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Aboutuspages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Paragraph1,Paragraph2,Videourl,Backgroundvideoimg")] Aboutuspage aboutuspage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aboutuspage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aboutuspage);
        }

        // GET: Aboutuspages/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Aboutuspages == null)
            {
                return NotFound();
            }

            var aboutuspage = await _context.Aboutuspages.FindAsync(id);
            if (aboutuspage == null)
            {
                return NotFound();
            }
            return View(aboutuspage);
        }

        // POST: Aboutuspages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Title,Paragraph1,Paragraph2,Videourl,ImageFile")] Aboutuspage aboutuspage)
        {
            if (id != aboutuspage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (aboutuspage.ImageFile != null)
                    {
                        string wwwRootpath = _webHostEnvironment.WebRootPath;
                        string filename = Guid.NewGuid().ToString() + "_" + aboutuspage.ImageFile.FileName;
                        string path = Path.Combine(wwwRootpath + "/images/", filename);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await aboutuspage.ImageFile.CopyToAsync(fileStream);
                        }
                        aboutuspage.Backgroundvideoimg = filename;
                    }


                    _context.Update(aboutuspage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AboutuspageExists(aboutuspage.Id))
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
            return View(aboutuspage);
        }

        // GET: Aboutuspages/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Aboutuspages == null)
            {
                return NotFound();
            }

            var aboutuspage = await _context.Aboutuspages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aboutuspage == null)
            {
                return NotFound();
            }

            return View(aboutuspage);
        }

        // POST: Aboutuspages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Aboutuspages == null)
            {
                return Problem("Entity set 'ModelContext.Aboutuspages'  is null.");
            }
            var aboutuspage = await _context.Aboutuspages.FindAsync(id);
            if (aboutuspage != null)
            {
                _context.Aboutuspages.Remove(aboutuspage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AboutuspageExists(decimal id)
        {
            return (_context.Aboutuspages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
