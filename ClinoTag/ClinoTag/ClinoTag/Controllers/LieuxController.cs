using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClinoTag.Models;

namespace ClinoTag.Controllers
{
    public class LieuxController : Controller
    {
        private readonly CLINOTAGContext _context;

        public LieuxController(CLINOTAGContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Lieux/SupprTacheLieu/{idTacheLieu}")]
        public async Task<IActionResult> SupprTacheLieu(int idTacheLieu)
        {
            TacheLieu tl = await _context.TacheLieus.FindAsync(idTacheLieu);
            int idLieu = tl.IdLieu;
            _context.TacheLieus.Remove(tl);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Lieux", new { id = idLieu });
        }

        [HttpGet]
        [Route("Lieux/AjoutTacheLieu/{idLieu}/{idTache}")]
        public async Task<bool> AjoutTacheLieu(int idLieu, int idTache)
        {
            try
            {
                TacheLieu newTL = new TacheLieu();
                newTL.IdLieu = idLieu;
                newTL.IdTache = idTache;
                _context.TacheLieus.Add(newTL);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // GET: Lieux
        public async Task<IActionResult> Index()
        {
            var cLINOTAGContext = _context.Lieus
                .Include(l => l.IdClientNavigation)
                .Include(l => l.TacheLieus);
            return View(await cLINOTAGContext.ToListAsync());
        }

        // GET: Lieux/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lieus == null)
            {
                return NotFound();
            }

            var lieu = await _context.Lieus
                .Include(l => l.IdClientNavigation)
                .Include(l => l.TacheLieus).ThenInclude(t => t.IdTacheNavigation)
                .FirstOrDefaultAsync(m => m.IdLieu == id);
            if (lieu == null)
            {
                return NotFound();
            }

            return View(lieu);
        }

        // GET: Lieux/Create
        public IActionResult Create()
        {
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "Nom");
            return View();
        }

        // POST: Lieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLieu,Nom,UidTag,IdClient")] Lieu lieu)
        {
            if (lieu.IdClient > 0 && lieu.UidTag != null)
            {
                _context.Add(lieu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "Nom", lieu.IdClient);
            return View(lieu);
        }

        // GET: Lieux/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Lieus == null) return NotFound();

            var lieu = await _context.Lieus
                .Include(l => l.TacheLieus).ThenInclude(tl => tl.IdTacheNavigation)
                .Include(l => l.TacheLieus).ThenInclude(tl => tl.PassageTaches)
                .Where(l => l.IdLieu == id)
                .FirstOrDefaultAsync();

            if (lieu == null) return NotFound();

            var excludedIDs = new HashSet<int>(lieu.TacheLieus.Select(p => p.IdTache));
            ViewBag.ListeTache = await _context.Taches.Where(t => !excludedIDs.Contains(t.IdTache)).ToListAsync();

            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "Nom", lieu.IdClient);
            return View(lieu);
        }

        // POST: Lieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLieu,Nom,UidTag,IdClient")] Lieu lieu)
        {
            if (id != lieu.IdLieu)
            {
                return NotFound();
            }

            if (lieu.IdClient > 0 && lieu.UidTag != null)
            {
                try
                {
                    _context.Update(lieu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LieuExists(lieu.IdLieu))
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
            ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "Nom", lieu.IdClient);
            return View(lieu);
        }

        // GET: Lieux/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Lieus == null)
            {
                return NotFound();
            }

            var lieu = await _context.Lieus
                .Include(l => l.IdClientNavigation)
                .FirstOrDefaultAsync(m => m.IdLieu == id);
            if (lieu == null)
            {
                return NotFound();
            }

            return View(lieu);
        }

        // POST: Lieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Lieus == null)
            {
                return Problem("Entity set 'CLINOTAGContext.Lieus'  is null.");
            }
            var lieu = await _context.Lieus.FindAsync(id);
            if (lieu != null)
            {
                _context.Lieus.Remove(lieu);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LieuExists(int id)
        {
          return _context.Lieus.Any(e => e.IdLieu == id);
        }
    }
}
