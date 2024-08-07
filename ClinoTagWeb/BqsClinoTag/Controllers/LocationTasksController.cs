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
    public class LocationTasksController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public LocationTasksController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        // GET: LocationTasks
        public async Task<IActionResult> Index()
        {
              return View(await _context.TacheLieus.Include(x => x.IdTacheNavigation).Include( x => x.IdLieuNavigation).ToListAsync());
        }

        // GET: LocationTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Taches == null)
            {
                return NotFound();
            }

            var locationTask = await _context.TacheLieus
                .FirstOrDefaultAsync(m => m.IdTl == id);

            if (locationTask == null)
            {
                return NotFound();
            }

            return View(locationTask);
        }

        // GET: LocationTasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Taches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTache,IdLieu")] TacheLieu tache)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tache);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tache);
        }

        // GET: Taches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TacheLieus == null)
            {
                return NotFound();
            }

            var tache = await _context.TacheLieus.FindAsync(id);
            if (tache == null)
            {
                return NotFound();
            }
            return View(tache);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTache,IdLieu")] Tache tache)
        {
            if (id != tache.IdTache)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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

        // GET: LocationTask/Clone/5
        public async Task<IActionResult> Clone(int? id)
        {
            if (id == null || _context.TacheLieus == null)
            {
                return NotFound();
            }

            var tache = await _context.TacheLieus
                .FirstOrDefaultAsync(m => m.IdTl == id);

            var task = new TacheLieu();

            task.IdLieu = tache.IdLieu;
            task.IdTache = tache.IdTache;

            _context.TacheLieus.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Taches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Taches == null)
            {
                return NotFound();
            }

            var tache = await _context.TacheLieus
                .FirstOrDefaultAsync(m => m.IdTl == id);
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
            if (_context.TacheLieus == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Taches'  is null.");
            }
            var tache = await _context.TacheLieus.FindAsync(id);
            if (tache != null)
            {
                _context.TacheLieus.Remove(tache);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TacheExists(int id)
        {
          return _context.TacheLieus.Any(e => e.IdTl == id);
        }
    }
}
