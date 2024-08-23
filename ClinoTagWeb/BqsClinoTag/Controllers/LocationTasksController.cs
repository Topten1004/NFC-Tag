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
            if (id == null || _context.TacheLieus == null)
            {
                return NotFound();
            }

            var locationTask = await _context.TacheLieus.Include(x => x.IdLieuNavigation).Include(x => x.IdTacheNavigation)
                .FirstOrDefaultAsync(m => m.IdTl == id);

            if (locationTask == null)
            {
                return NotFound();
            }

            return View(locationTask);
        }

        // GET: LocationTasks/Create
        public async Task<IActionResult> CreateAsync()
        {
            ViewBag.Tasks = await _context.Taches
                .Select(t => new SelectListItem
                {
                    Value = t.IdTache.ToString(),
                    Text = t.Nom
                }).ToListAsync();

            ViewBag.Locations = await _context.Lieus
               .Select(l => new SelectListItem
               {
                   Value = l.IdLieu.ToString(),
                   Text = l.Nom
               }).ToListAsync();

            return View();
        }

        // POST: Taches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdTache,IdLieu")] TacheLieu tache)
        {

            if(tache.IdTache > 0 && tache.IdLieu > 0)
            {
                _context.Add(tache);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(tache);
        }

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

            // Pre-select the current task
            ViewBag.Tasks = await _context.Taches
                .Select(t => new SelectListItem
                {
                    Value = t.IdTache.ToString(),
                    Text = t.Nom,
                    Selected = t.IdTache == tache.IdTache // This sets the selected item
                }).ToListAsync();

            // Pre-select the current location
            ViewBag.Locations = await _context.Lieus
                .Select(l => new SelectListItem
                {
                    Value = l.IdLieu.ToString(),
                    Text = l.Nom,
                    Selected = l.IdLieu == tache.IdLieu // This sets the selected item
                }).ToListAsync();

            return View(tache);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdTache,IdLieu")] TacheLieu tache)
        {
            if (tache.IdLieu > 0 && tache.IdTache > 0)
            {
                try
                {
                    var task = _context.TacheLieus.Include(x => x.IdLieuNavigation).Include(x => x.IdTacheNavigation).Where(x => x.IdTl == id).FirstOrDefault();

                    task.IdLieu = tache.IdLieu;
                    task.IdTache = tache.IdTache;

                    _context.Update(task);
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

        [HttpPost]
        public async Task<IActionResult> Import(int id, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var fileBytes = ms.ToArray();
                    var base64String = Convert.ToBase64String(fileBytes);

                    // Find the TacheLieu by ID
                    var tacheLieu = await _context.TacheLieus.FindAsync(id);
                    if (tacheLieu != null)
                    {
                        tacheLieu.Photo = fileBytes; // Save as byte[] in the database
                        await _context.SaveChangesAsync();
                    }
                }
            }

            return RedirectToAction(nameof(Index));
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
            if (id == null || _context.TacheLieus == null)
            {
                return NotFound();
            }

            var tache = await _context.TacheLieus.Include(x => x.IdTacheNavigation).Include(x => x.IdLieuNavigation)
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
