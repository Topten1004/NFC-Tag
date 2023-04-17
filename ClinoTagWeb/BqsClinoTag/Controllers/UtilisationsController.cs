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

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN) + "," + nameof(Droits.Roles.MANAGER))]
    public class UtilisationsController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public UtilisationsController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Utilisations/GeolocUtilisation/{idUtilisation}")]
        public async Task<Tuple<double, double>?> GeolocUtilisation(int idUtilisation)
        {
            Utilisation? u = await _context.Utilisations.FindAsync(idUtilisation);
            if (u == null) return null;

            GeolocAgent? ga = await _context.GeolocAgents
                .Where(g => g.IdAgent == u.IdAgent 
                            && g.DhGeoloc <= u.DhDebut.AddMinutes(30) && g.DhGeoloc >= u.DhDebut.AddMinutes(-30))
                .OrderByDescending(g => g.DhGeoloc)
                .FirstOrDefaultAsync();

            if (ga != null)
                return new Tuple<double, double>(ga.Lati, ga.Longi);
            else
                return null;
        }

        // GET: Utilisations
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
                    return View(await _context.Utilisations
                        .Where(p => clientsID.Contains(p.IdMaterielNavigation.IdClient)
                                    && p.DhDebut.Date <= dtFin.Date && (!p.DhFin.HasValue || p.DhFin.Value.Date >= dtDebut.Date))
                        .Include(p => p.IdAgentNavigation)
                        .Include(p => p.IdMaterielNavigation).ThenInclude(l => l.IdClientNavigation)
                        .OrderByDescending(p => p.DhDebut).ToListAsync());
                default:
                    HttpContext.Session.SetInt32("idClient", idClient);
                    List<Client> lC = await _context.Clients.ToListAsync();
                    Client c = new Client { Nom = "<tous>", IdClient = 0 };
                    lC.Insert(0, c);
                    ViewData["IdClient"] = new SelectList(lC, "IdClient", "Nom", HttpContext.Session.GetInt32("idClient"));

                    return View(await _context.Utilisations
                        .Where(p => (idClient == 0 || p.IdMaterielNavigation.IdClient == idClient) &&
                                    p.DhDebut.Date <= dtFin.Date && (!p.DhFin.HasValue || p.DhFin.Value.Date >= dtDebut.Date))
                        .Include(p => p.IdAgentNavigation)
                        .Include(p => p.IdMaterielNavigation).ThenInclude(l => l.IdClientNavigation)
                        .OrderByDescending(p => p.DhDebut).ToListAsync());
            }
        }

        // GET: Utilisations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Utilisations == null)
            {
                return NotFound();
            }

            var utilisation = await _context.Utilisations
                .Include(u => u.IdAgentNavigation)
                .Include(u => u.IdMaterielNavigation)
                .FirstOrDefaultAsync(m => m.IdUtilisation == id);
            if (utilisation == null)
            {
                return NotFound();
            }

            return View(utilisation);
        }

        // GET: Utilisations/Create
        public IActionResult Create()
        {
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "IdAgent");
            ViewData["IdMateriel"] = new SelectList(_context.Materiels, "IdMateriel", "IdMateriel");
            return View();
        }

        // POST: Utilisations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUtilisation,DhDebut,DhFin,IdMateriel,IdAgent,Commentaire")] Utilisation utilisation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utilisation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "IdAgent", utilisation.IdAgent);
            ViewData["IdMateriel"] = new SelectList(_context.Materiels, "IdMateriel", "IdMateriel", utilisation.IdMateriel);
            return View(utilisation);
        }

        // GET: Utilisations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Utilisations == null)
            {
                return NotFound();
            }

            var utilisation = await _context.Utilisations.FindAsync(id);
            if (utilisation == null)
            {
                return NotFound();
            }
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "IdAgent", utilisation.IdAgent);
            ViewData["IdMateriel"] = new SelectList(_context.Materiels, "IdMateriel", "IdMateriel", utilisation.IdMateriel);
            return View(utilisation);
        }

        // POST: Utilisations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUtilisation,DhDebut,DhFin,IdMateriel,IdAgent,Commentaire")] Utilisation utilisation)
        {
            if (id != utilisation.IdUtilisation)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilisation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilisationExists(utilisation.IdUtilisation))
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
            ViewData["IdAgent"] = new SelectList(_context.Agents, "IdAgent", "IdAgent", utilisation.IdAgent);
            ViewData["IdMateriel"] = new SelectList(_context.Materiels, "IdMateriel", "IdMateriel", utilisation.IdMateriel);
            return View(utilisation);
        }

        // GET: Utilisations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Utilisations == null)
            {
                return NotFound();
            }

            var utilisation = await _context.Utilisations
                .Include(u => u.IdAgentNavigation)
                .Include(u => u.IdMaterielNavigation)
                .FirstOrDefaultAsync(m => m.IdUtilisation == id);
            if (utilisation == null)
            {
                return NotFound();
            }

            return View(utilisation);
        }

        // POST: Utilisations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Utilisations == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Utilisations'  is null.");
            }
            var utilisation = await _context.Utilisations.FindAsync(id);
            if (utilisation != null)
            {
                _context.Utilisations.Remove(utilisation);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilisationExists(int id)
        {
          return _context.Utilisations.Any(e => e.IdUtilisation == id);
        }
    }
}
