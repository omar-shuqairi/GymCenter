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
    public class MembersController : Controller
    {
        private readonly ModelContext _context;

        public MembersController(ModelContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Members.Include(m => m.Plan).Include(m => m.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Plan)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Memberid == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            ViewData["Planid"] = new SelectList(_context.Workoutplans, "Planid", "Planid");
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid");
            return View();
        }

        // POST: Members/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Memberid,SubscriptionStart,SubscriptionEnd,Planid,Userid")] Member member)
        {
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Planid"] = new SelectList(_context.Workoutplans, "Planid", "Planid", member.Planid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", member.Userid);
            return View(member);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members.FindAsync(id);
            if (member == null)
            {
                return NotFound();
            }
            ViewData["Planid"] = new SelectList(_context.Workoutplans, "Planid", "Planid", member.Planid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", member.Userid);
            return View(member);
        }

        // POST: Members/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Memberid,SubscriptionStart,SubscriptionEnd,Planid,Userid")] Member member)
        {
            if (id != member.Memberid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberExists(member.Memberid))
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
            ViewData["Planid"] = new SelectList(_context.Workoutplans, "Planid", "Planid", member.Planid);
            ViewData["Userid"] = new SelectList(_context.Users, "Userid", "Userid", member.Userid);
            return View(member);
        }

        // GET: Members/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Members == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.Plan)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Memberid == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Members/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Members == null)
            {
                return Problem("Entity set 'ModelContext.Members'  is null.");
            }
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberExists(decimal id)
        {
          return (_context.Members?.Any(e => e.Memberid == id)).GetValueOrDefault();
        }
    }
}
