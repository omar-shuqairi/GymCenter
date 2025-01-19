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
    public class ContactformsController : Controller
    {
        private readonly ModelContext _context;

        public ContactformsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Contactforms
        public async Task<IActionResult> Index()
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            return _context.Contactforms != null ?
                          View(await _context.Contactforms.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Contactforms'  is null.");
        }

        // GET: Contactforms/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Contactforms == null)
            {
                return NotFound();
            }

            var contactform = await _context.Contactforms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactform == null)
            {
                return NotFound();
            }

            return View(contactform);
        }

        // GET: Contactforms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contactforms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Guestname,Guestemail,Guestcomment")] Contactform contactform)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contactform);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contactform);
        }

        // GET: Contactforms/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Contactforms == null)
            {
                return NotFound();
            }

            var contactform = await _context.Contactforms.FindAsync(id);
            if (contactform == null)
            {
                return NotFound();
            }
            return View(contactform);
        }

        // POST: Contactforms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,Guestname,Guestemail,Guestcomment")] Contactform contactform)
        {
            if (id != contactform.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contactform);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactformExists(contactform.Id))
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
            return View(contactform);
        }

        // GET: Contactforms/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            ViewData["AdmimFullName"] = HttpContext.Session.GetString("AdminFullName");
            ViewData["AdminEmail"] = HttpContext.Session.GetString("AdminEmail");
            ViewData["AdminImg"] = HttpContext.Session.GetString("AdminImg");
            if (id == null || _context.Contactforms == null)
            {
                return NotFound();
            }

            var contactform = await _context.Contactforms
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contactform == null)
            {
                return NotFound();
            }

            return View(contactform);
        }

        // POST: Contactforms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Contactforms == null)
            {
                return Problem("Entity set 'ModelContext.Contactforms'  is null.");
            }
            var contactform = await _context.Contactforms.FindAsync(id);
            if (contactform != null)
            {
                _context.Contactforms.Remove(contactform);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactformExists(decimal id)
        {
            return (_context.Contactforms?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
