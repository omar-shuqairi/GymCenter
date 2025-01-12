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
    public class WhychooseusController : Controller
    {
        private readonly ModelContext _context;

        public WhychooseusController(ModelContext context)
        {
            _context = context;
        }

        // GET: Whychooseus
        public async Task<IActionResult> Index()
        {
              return _context.Whychooseus != null ? 
                          View(await _context.Whychooseus.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Whychooseus'  is null.");
        }

        // GET: Whychooseus/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Whychooseus == null)
            {
                return NotFound();
            }

            var whychooseu = await _context.Whychooseus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (whychooseu == null)
            {
                return NotFound();
            }

            return View(whychooseu);
        }

        // GET: Whychooseus/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Whychooseus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IconClass,Title,Description")] Whychooseu whychooseu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(whychooseu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(whychooseu);
        }

        // GET: Whychooseus/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Whychooseus == null)
            {
                return NotFound();
            }

            var whychooseu = await _context.Whychooseus.FindAsync(id);
            if (whychooseu == null)
            {
                return NotFound();
            }
            return View(whychooseu);
        }

        // POST: Whychooseus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,IconClass,Title,Description")] Whychooseu whychooseu)
        {
            if (id != whychooseu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(whychooseu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WhychooseuExists(whychooseu.Id))
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
            return View(whychooseu);
        }

        // GET: Whychooseus/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Whychooseus == null)
            {
                return NotFound();
            }

            var whychooseu = await _context.Whychooseus
                .FirstOrDefaultAsync(m => m.Id == id);
            if (whychooseu == null)
            {
                return NotFound();
            }

            return View(whychooseu);
        }

        // POST: Whychooseus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Whychooseus == null)
            {
                return Problem("Entity set 'ModelContext.Whychooseus'  is null.");
            }
            var whychooseu = await _context.Whychooseus.FindAsync(id);
            if (whychooseu != null)
            {
                _context.Whychooseus.Remove(whychooseu);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WhychooseuExists(decimal id)
        {
          return (_context.Whychooseus?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
