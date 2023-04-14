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
    public class PassagesController : Controller
    {
        private readonly CLINOTAGContext _context;

        public PassagesController(CLINOTAGContext context)
        {
            _context = context;
        }

        // GET: Passages
        public async Task<IActionResult> Index()
        {
            var cLINOTAGContext = _context.Passages
                .Include(p => p.IdAgentNavigation)
                .Include(p => p.IdLieuNavigation).ThenInclude(l => l.IdClientNavigation)
                .Include(p => p.PassageTaches).ThenInclude(t => t.IdTlNavigation).ThenInclude(tl => tl.IdTacheNavigation)
                .OrderByDescending(p => p.DhDebut);
            return View(await cLINOTAGContext.ToListAsync());
        }

        // GET: Passages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Passages == null) return NotFound();

            var passage = await _context.Passages
                .Include(p => p.IdAgentNavigation)
                .Include(p => p.IdLieuNavigation)
                .Include(p => p.PassageTaches).ThenInclude(pt => pt.IdTlNavigation).ThenInclude(tl => tl.IdTacheNavigation)
                .FirstOrDefaultAsync(m => m.IdPassage == id);

            if (passage == null) return NotFound();

            return View(passage);
        }

        // GET: Passages/Create
        public IActionResult Create()
        {
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "Nom");
            ViewData["IdLieu"] = new SelectList(_context.Lieus, "IdLieu", "Nom");
            return View();
        }

        // POST: Passages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdPassage,IdLieu,IdAgent,DhDebut,DhFin")] Passage passage)
        {
            if (passage.IdLieu > 0 && passage.IdAgent > 0)
            {
                _context.Add(passage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "Nom", passage.IdAgent);
            ViewData["IdLieu"] = new SelectList(_context.Lieus, "IdLieu", "Nom", passage.IdLieu);
            return View(passage);
        }

        // GET: Passages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Passages == null)
            {
                return NotFound();
            }

            var passage = await _context.Passages.FindAsync(id);
            if (passage == null)
            {
                return NotFound();
            }
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "Nom", passage.IdAgent);
            ViewData["IdLieu"] = new SelectList(_context.Lieus, "IdLieu", "Nom", passage.IdLieu);
            return View(passage);
        }

        // POST: Passages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdPassage,IdLieu,IdAgent,DhDebut,DhFin")] Passage passage)
        {
            if (id != passage.IdPassage)
            {
                return NotFound();
            }


            try
            {
                _context.Update(passage);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PassageExists(passage.IdPassage))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            
            //ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "Nom", passage.IdAgent);
            //ViewData["IdLieu"] = new SelectList(_context.Lieus, "IdLieu", "Nom", passage.IdLieu);
            //return View(passage);
        }

        // GET: Passages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Passages == null)
            {
                return NotFound();
            }

            var passage = await _context.Passages
                .Include(p => p.IdAgentNavigation)
                .Include(p => p.IdLieuNavigation)
                .FirstOrDefaultAsync(m => m.IdPassage == id);
            if (passage == null)
            {
                return NotFound();
            }

            return View(passage);
        }

        // POST: Passages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Passages == null)
            {
                return Problem("Entity set 'CLINOTAGContext.Passages'  is null.");
            }
            var passage = await _context.Passages.FindAsync(id);
            if (passage != null)
            {
                _context.Passages.Remove(passage);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PassageExists(int id)
        {
          return _context.Passages.Any(e => e.IdPassage == id);
        }
    }
}
