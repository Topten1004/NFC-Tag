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
using System.Globalization;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN) + "," + nameof(Droits.Roles.MANAGER))]
    public class MaterielsController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public MaterielsController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        // GET: Materiels
        public async Task<IActionResult> Index(int idClient = 0)
        {
            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            switch (userRole)
            {
                case nameof(Droits.Roles.MANAGER):
                    var clientsID = new HashSet<int>(_context.Uclients.Where(u => u.IdUtilisateur == idUtilisateur).Select(p => p.IdClient));
                    return View(await _context.Materiels
                        .Where(m => clientsID.Contains(m.IdClient))
                        .Include(m => m.IdClientNavigation)
                        .ToListAsync());
                default:
                    HttpContext.Session.SetInt32("idClient", idClient);
                    List<Client> lC = await _context.Clients.ToListAsync();
                    Client c = new Client { Nom = "<tous>", IdClient = 0 };
                    lC.Insert(0, c);
                    ViewData["IdClient"] = new SelectList(lC, "IdClient", "Nom", HttpContext.Session.GetInt32("idClient"));
                    return View(await _context.Materiels
                        .Include(o => o.IdClientNavigation)
                        .Where(m => idClient == 0 || m.IdClient == idClient)
                        .ToListAsync());
            }            
        }

        // GET: Materiels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Materiels == null || !await AutoriseUtilisateur((int)id)) return NotFound();

            var materiel = await _context.Materiels
                .Include(o => o.IdClientNavigation)
                .FirstOrDefaultAsync(m => m.IdMateriel == id);
            if (materiel == null) return NotFound();

            return View(materiel);
        }

        // GET: Materiels/Create
        public IActionResult Create()
        {
            chargeListeCLient();
            return View();
        }

        private void chargeListeCLient()
        {
            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            switch (userRole)
            {
                case nameof(Droits.Roles.MANAGER):
                    var clientsID = new HashSet<int>(_context.Uclients.Where(u => u.IdUtilisateur == idUtilisateur).Select(p => p.IdClient));
                    ViewData["IdClient"] = new SelectList(_context.Clients.Where(c => clientsID.Contains(c.IdClient)), "IdClient", "Nom");
                    break;
                default:
                    ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "Nom");
                    break;
            }
        }

        // POST: Materiels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMateriel,Nom,Instruction,Expiration,IdClient,UidTag")] Materiel materiel)
        {
            try
            {
                materiel.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(materiel.Nom.ToLower());
                _context.Add(materiel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "Nom", materiel.IdClient);
                return View(materiel);
            }            
        }

        // GET: Materiels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Materiels == null || !await AutoriseUtilisateur((int)id))
            {
                return NotFound();
            }

            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel == null)
            {
                return NotFound();
            }
            chargeListeCLient();
            return View(materiel);
        }

        // POST: Materiels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMateriel,Nom,Instruction,Expiration,IdClient,UidTag")] Materiel materiel)
        {
            if (id != materiel.IdMateriel)
            {
                return NotFound();
            }

            try
            {
                materiel.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(materiel.Nom.ToLower());
                _context.Update(materiel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterielExists(materiel.IdMateriel))
                {
                    return NotFound();
                }
                else
                {
                    ViewData["IdClient"] = new SelectList(_context.Clients, "IdClient", "Nom", materiel.IdClient);
                    return View(materiel);
                }
            }  
        }

        // GET: Materiels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Materiels == null || !await AutoriseUtilisateur((int)id))
            {
                return NotFound();
            }

            var materiel = await _context.Materiels
                .Include(o => o.IdClientNavigation)
                .FirstOrDefaultAsync(m => m.IdMateriel == id);
            if (materiel == null)
            {
                return NotFound();
            }

            return View(materiel);
        }

        private async Task<bool> AutoriseUtilisateur(int idMateriel)
        {
            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == nameof(Droits.Roles.SUPERADMIN) || userRole == nameof(Droits.Roles.ADMIN)) return true;

            Materiel? m = await _context.Materiels.FindAsync(idMateriel);
            if (m == null) return false;
            int idClient = m.IdClient;
            
            int idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var clientsID = new HashSet<int>(_context.Uclients.Where(u => u.IdUtilisateur == idUtilisateur).Select(p => p.IdClient));
            if (clientsID.Contains(idClient)) return true;
            else return false;
        }

        // POST: Materiels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Materiels == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Materiels'  is null.");
            }
            var materiel = await _context.Materiels.FindAsync(id);
            if (materiel != null && await AutoriseUtilisateur(id))
            {
                _context.Materiels.Remove(materiel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaterielExists(int id)
        {
          return _context.Materiels.Any(e => e.IdMateriel == id);
        }
    }
}
