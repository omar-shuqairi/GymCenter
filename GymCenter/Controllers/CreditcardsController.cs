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
    public class CreditcardsController : Controller
    {
        private readonly ModelContext _context;

        public CreditcardsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Creditcards
        public async Task<IActionResult> Index()
        {
              return _context.Creditcards != null ? 
                          View(await _context.Creditcards.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Creditcards'  is null.");
        }

        // GET: Creditcards/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Creditcards == null)
            {
                return NotFound();
            }

            var creditcard = await _context.Creditcards
                .FirstOrDefaultAsync(m => m.Cardid == id);
            if (creditcard == null)
            {
                return NotFound();
            }

            return View(creditcard);
        }

        // GET: Creditcards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Creditcards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Cardid,Cardnumber,Cvv,Balance,Expirydate")] Creditcard creditcard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(creditcard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(creditcard);
        }

        // GET: Creditcards/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Creditcards == null)
            {
                return NotFound();
            }

            var creditcard = await _context.Creditcards.FindAsync(id);
            if (creditcard == null)
            {
                return NotFound();
            }
            return View(creditcard);
        }

        // POST: Creditcards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Cardid,Cardnumber,Cvv,Balance,Expirydate")] Creditcard creditcard)
        {
            if (id != creditcard.Cardid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creditcard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreditcardExists(creditcard.Cardid))
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
            return View(creditcard);
        }

        // GET: Creditcards/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Creditcards == null)
            {
                return NotFound();
            }

            var creditcard = await _context.Creditcards
                .FirstOrDefaultAsync(m => m.Cardid == id);
            if (creditcard == null)
            {
                return NotFound();
            }

            return View(creditcard);
        }

        // POST: Creditcards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Creditcards == null)
            {
                return Problem("Entity set 'ModelContext.Creditcards'  is null.");
            }
            var creditcard = await _context.Creditcards.FindAsync(id);
            if (creditcard != null)
            {
                _context.Creditcards.Remove(creditcard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CreditcardExists(decimal id)
        {
          return (_context.Creditcards?.Any(e => e.Cardid == id)).GetValueOrDefault();
        }
    }
}
