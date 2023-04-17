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
using System.Security.Claims;
using System.Collections;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN) + "," + nameof(Droits.Roles.MANAGER))]
    public class PassagesController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public PassagesController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        // GET: Passages
        public async Task<IActionResult> Index(DateTime dtDebut, DateTime dtFin, int idClient = 0)
        {
            if (dtDebut == DateTime.MinValue || dtFin == DateTime.MinValue)
            {
                if (HttpContext.Session.GetString("dtDebut") == null) dtDebut = DateTime.Now.AddMonths(-1);
                else dtDebut = Convert.ToDateTime(HttpContext.Session.GetString("dtDebut"));
                if (HttpContext.Session.GetString("dtFin") == null) dtFin = DateTime.Now.AddMonths(1);
                else dtFin = Convert.ToDateTime(HttpContext.Session.GetString("dtFin"));                
            }
            HttpContext.Session.SetString("dtDebut", dtDebut.Date.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("dtFin", dtFin.Date.ToString("yyyy-MM-dd"));
            ViewBag.dtDebut = dtDebut;
            ViewBag.dtFin = dtFin;

            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            switch (userRole)
            {
                case nameof(Droits.Roles.MANAGER):
                    var clientsID = new HashSet<int>(_context.Uclients.Where(u => u.IdUtilisateur == idUtilisateur).Select(p => p.IdClient));
                    return View(await _context.Passages
                        .Where(p=> clientsID.Contains(p.IdLieuNavigation.IdClient) 
                                    && p.DhDebut.Date <= dtFin.Date && p.DhFin.Date >= dtDebut.Date)
                        .Include(p => p.IdAgentNavigation)
                        .Include(p => p.IdLieuNavigation).ThenInclude(l => l.IdClientNavigation)
                        .Include(p => p.PassageTaches).ThenInclude(t => t.IdTlNavigation).ThenInclude(tl => tl.IdTacheNavigation)
                        .OrderByDescending(p => p.DhDebut).ToListAsync());
                default:
                    HttpContext.Session.SetInt32("idClient", idClient);
                    List<Client> lC = await _context.Clients.ToListAsync();
                    Client c = new Client { Nom = "<tous>", IdClient = 0 };
                    lC.Insert(0, c);
                    ViewData["IdClient"] = new SelectList(lC, "IdClient", "Nom", HttpContext.Session.GetInt32("idClient"));


                    return View(await _context.Passages
                        .Where(p => (idClient == 0 || p.IdLieuNavigation.IdClient == idClient) &&
                                    p.DhDebut.Date <= dtFin.Date && p.DhFin.Date >= dtDebut.Date)
                        .Include(p => p.IdAgentNavigation)
                        .Include(p => p.IdLieuNavigation).ThenInclude(l => l.IdClientNavigation)
                        .Include(p => p.PassageTaches).ThenInclude(t => t.IdTlNavigation).ThenInclude(tl => tl.IdTacheNavigation)
                        .OrderByDescending(p => p.DhDebut).ToListAsync());
            }
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
        [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
        public IActionResult Create()
        {
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "Nom");
            ViewData["IdLieu"] = new SelectList(ListeClientsLieux(), "IdLieu", "ClientLieu");
            return View();
        }

        // POST: Passages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
        public async Task<IActionResult> Create([Bind("IdPassage,IdLieu,IdAgent,DhDebut,DhFin")] Passage passage)
        {
            if (passage.IdLieu > 0 && passage.IdAgent > 0)
            {
                _context.Add(passage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "Nom", passage.IdAgent);
            ViewData["IdLieu"] = new SelectList(ListeClientsLieux(), "IdLieu", "ClientLieu", passage.IdLieu);
            return View(passage);
        }

        [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
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
            ViewData["IdLieu"] = new SelectList(ListeClientsLieux(), "IdLieu", "ClientLieu", passage.IdLieu);
            return View(passage);
        }

        // POST: Passages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
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
                return RedirectToAction(nameof(Index));
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
            
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "Nom", passage.IdAgent);
            ViewData["IdLieu"] = new SelectList(ListeClientsLieux(), "IdLieu", "ClientLieu", passage.IdLieu);
            return View(passage);
        }

        private IEnumerable ListeClientsLieux()
        {
            return _context.Lieus.Include(l => l.IdClientNavigation).OrderBy(l => l.IdClientNavigation.Nom);
        }

        // GET: Passages/Delete/5
        [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
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
        [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Passages == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Passages'  is null.");
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
