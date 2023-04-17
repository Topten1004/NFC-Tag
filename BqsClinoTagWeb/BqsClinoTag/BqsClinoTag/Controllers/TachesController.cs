using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BqsClinoTag.Models;
using Microsoft.AspNetCore.Authorization;
using BqsClinoTag.Grool;
using System.Globalization;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
    public class TachesController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public TachesController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        // GET: Taches
        public async Task<IActionResult> Index()
        {
              return View(await _context.Taches.ToListAsync());
        }

        // GET: Taches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Taches == null)
            {
                return NotFound();
            }

            var tache = await _context.Taches
                .FirstOrDefaultAsync(m => m.IdTache == id);
            if (tache == null)
            {
                return NotFound();
            }

            return View(tache);
        }

        // GET: Taches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Taches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTache,Nom,Description")] Tache tache)
        {
            if (ModelState.IsValid)
            {
                tache.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tache.Nom.ToLower());
                _context.Add(tache);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tache);
        }

        // GET: Taches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Taches == null)
            {
                return NotFound();
            }

            var tache = await _context.Taches.FindAsync(id);
            if (tache == null)
            {
                return NotFound();
            }
            return View(tache);
        }

        // POST: Taches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTache,Nom,Description")] Tache tache)
        {
            if (id != tache.IdTache)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tache.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tache.Nom.ToLower());
                    _context.Update(tache);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TacheExists(tache.IdTache))
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
            return View(tache);
        }

        // GET: Taches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Taches == null)
            {
                return NotFound();
            }

            var tache = await _context.Taches
                .FirstOrDefaultAsync(m => m.IdTache == id);
            if (tache == null)
            {
                return NotFound();
            }

            return View(tache);
        }

        // POST: Taches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Taches == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Taches'  is null.");
            }
            var tache = await _context.Taches.FindAsync(id);
            if (tache != null)
            {
                _context.Taches.Remove(tache);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TacheExists(int id)
        {
          return _context.Taches.Any(e => e.IdTache == id);
        }
    }
}
