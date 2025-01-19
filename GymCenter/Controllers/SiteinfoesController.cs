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
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            return _context.Siteinfos != null ?
                        View(await _context.Siteinfos.ToListAsync()) :
                        Problem("Entity set 'ModelContext.Siteinfos'  is null.");
        }

        // GET: Siteinfoes/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
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
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
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
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
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

            var SiteinfoSaved = await _context.Siteinfos.FindAsync(id);
            if (id != siteinfo.Id)
            {
                return NotFound();
            }

            if (SiteinfoSaved == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    if (siteinfo.SiteImageFile != null)
                    {
                        string wwwRootpath = _webHostEnvironment.WebRootPath;
                        string filename = Guid.NewGuid().ToString() + "_" + siteinfo.SiteImageFile.FileName;
                        string path = Path.Combine(wwwRootpath + "/images/", filename);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await siteinfo.SiteImageFile.CopyToAsync(fileStream);
                        }
                        SiteinfoSaved.SiteImagePath = filename;
                    }
                    if (siteinfo.LogoImageFile != null)
                    {
                        string wwwRootpath = _webHostEnvironment.WebRootPath;
                        string filename = Guid.NewGuid().ToString() + "_" + siteinfo.LogoImageFile.FileName;
                        string path = Path.Combine(wwwRootpath + "/images/", filename);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await siteinfo.LogoImageFile.CopyToAsync(fileStream);
                        }
                        SiteinfoSaved.LogoImagePath = filename;
                    }
                    if (siteinfo.SharedImageFile != null)
                    {
                        string wwwRootpath = _webHostEnvironment.WebRootPath;
                        string filename = Guid.NewGuid().ToString() + "_" + siteinfo.SharedImageFile.FileName;
                        string path = Path.Combine(wwwRootpath + "/images/", filename);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await siteinfo.SharedImageFile.CopyToAsync(fileStream);
                        }
                        SiteinfoSaved.SharedImagePath = filename;
                    }

                    _context.Update(SiteinfoSaved);
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
        // GET: Siteinfoes/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
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
