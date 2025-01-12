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
    public class SiteinfoesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SiteinfoesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Siteinfoes
        public async Task<IActionResult> Index()
        {
            return _context.Siteinfos != null ?
                        View(await _context.Siteinfos.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Siteinfos'  is null.");
        }

        // GET: Siteinfoes/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Siteinfos == null)
            {
                return NotFound();
            }

            var siteinfo = await _context.Siteinfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteinfo == null)
            {
                return NotFound();
            }

            return View(siteinfo);
        }

        // GET: Siteinfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Siteinfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Sitename,SiteImagePath,LogoImagePath,SharedImagePath")] Siteinfo siteinfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(siteinfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(siteinfo);
        }

        // GET: Siteinfoes/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Siteinfos == null)
            {
                return NotFound();
            }

            var siteinfo = await _context.Siteinfos.FindAsync(id);
            if (siteinfo == null)
            {
                return NotFound();
            }
            return View(siteinfo);
        }

        // POST: Siteinfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Sitename,SiteImageFile,LogoImageFile,SharedImageFile")] Siteinfo siteinfo)
        {
            if (id != siteinfo.Id)
            {
                return NotFound();
            }

            var existingSiteinfo = await _context.Siteinfos.FindAsync(id);
            if (existingSiteinfo == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve existing image paths if no new files are uploaded and delete old images if new ones are uploaded
                    existingSiteinfo.Sitename = siteinfo.Sitename ?? existingSiteinfo.Sitename;

                    if (siteinfo.SiteImageFile != null)
                    {
                        // Delete old image
                        DeleteFile(existingSiteinfo.SiteImagePath);
                        // Upload new image
                        existingSiteinfo.SiteImagePath = await UploadFile(siteinfo.SiteImageFile);
                    }

                    if (siteinfo.LogoImageFile != null)
                    {
                        // Delete old logo
                        DeleteFile(existingSiteinfo.LogoImagePath);
                        // Upload new logo
                        existingSiteinfo.LogoImagePath = await UploadFile(siteinfo.LogoImageFile);
                    }

                    if (siteinfo.SharedImageFile != null)
                    {
                        // Delete old shared image
                        DeleteFile(existingSiteinfo.SharedImagePath);
                        // Upload new shared image
                        existingSiteinfo.SharedImagePath = await UploadFile(siteinfo.SharedImageFile);
                    }

                    // Save changes to the database
                    _context.Update(existingSiteinfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteinfoExists(siteinfo.Id))
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
            return View(siteinfo);
        }

        // Helper method to upload the file
        private async Task<string> UploadFile(IFormFile file)
        {
            string wwwRootpath = _webHostEnvironment.WebRootPath;
            string filename = Guid.NewGuid().ToString() + "_" + file.FileName;
            string path = Path.Combine(wwwRootpath + "/images/", filename);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return filename;
        }

        // Helper method to delete old file
        private void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            string wwwRootpath = _webHostEnvironment.WebRootPath;
            string fullPath = Path.Combine(wwwRootpath + "/images/", filePath);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
        // GET: Siteinfoes/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Siteinfos == null)
            {
                return NotFound();
            }

            var siteinfo = await _context.Siteinfos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (siteinfo == null)
            {
                return NotFound();
            }

            return View(siteinfo);
        }

        // POST: Siteinfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Siteinfos == null)
            {
                return Problem("Entity set 'ModelContext.Siteinfos'  is null.");
            }
            var siteinfo = await _context.Siteinfos.FindAsync(id);
            if (siteinfo != null)
            {
                _context.Siteinfos.Remove(siteinfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SiteinfoExists(decimal id)
        {
            return (_context.Siteinfos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
