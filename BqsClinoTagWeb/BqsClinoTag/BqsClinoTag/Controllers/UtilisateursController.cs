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
    public class UtilisateursController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public UtilisateursController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        // GET: Utilisateurs
        public async Task<IActionResult> Index()
        {
            var cLINOTAGBQSContext = _context.Utilisateurs.Include(u => u.RoleNavigation).Where(u => u.RoleNavigation.Role1 != "SUPERADMIN");
            return View(await cLINOTAGBQSContext.ToListAsync());
        }

        // GET: Utilisateurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Utilisateurs == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(m => m.IdUtilisateur == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // GET: Utilisateurs/Create
        public IActionResult Create()
        {
            ViewData["Role"] = new SelectList(_context.Roles.Where(r => r.Role1 != "SUPERADMIN"), "Role1", "Role1");
            return View();
        }

        // POST: Utilisateurs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUtilisateur,Nom,Prenom,Login,Email,Mdp,Role")] Utilisateur utilisateur)
        {
            try
            {
                utilisateur.Email = utilisateur.Email.ToLower();
                utilisateur.Login = utilisateur.Login.ToLower();
                utilisateur.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.Nom.ToLower());
                utilisateur.Prenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.Prenom.ToLower());
                _context.Add(utilisateur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewData["Role"] = new SelectList(_context.Roles.Where(r => r.Role1 != "SUPERADMIN"), "Role1", "Role1", utilisateur.Role);
                return View(utilisateur);
            }            
        }

        // GET: Utilisateurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Utilisateurs == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur == null)
            {
                return NotFound();
            }
            ViewData["Role"] = new SelectList(_context.Roles.Where(r => r.Role1 != "SUPERADMIN"), "Role1", "Role1", utilisateur.Role);
            return View(utilisateur);
        }

        // POST: Utilisateurs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUtilisateur,Nom,Prenom,Login,Email,Mdp,Role")] Utilisateur utilisateur)
        {
            if (id != utilisateur.IdUtilisateur) return NotFound();


            try
            {
                utilisateur.Email = utilisateur.Email.ToLower();
                utilisateur.Login = utilisateur.Login.ToLower();
                utilisateur.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.Nom.ToLower());
                utilisateur.Prenom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(utilisateur.Prenom.ToLower());
                _context.Update(utilisateur);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UtilisateurExists(utilisateur.IdUtilisateur))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
            
            ViewData["Role"] = new SelectList(_context.Roles.Where(r => r.Role1 != "SUPERADMIN"), "Role1", "Role1", utilisateur.Role);
            return View(utilisateur);
        }

        // GET: Utilisateurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Utilisateurs == null)
            {
                return NotFound();
            }

            var utilisateur = await _context.Utilisateurs
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(m => m.IdUtilisateur == id);
            if (utilisateur == null)
            {
                return NotFound();
            }

            return View(utilisateur);
        }

        // POST: Utilisateurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Utilisateurs == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Utilisateurs'  is null.");
            }
            var utilisateur = await _context.Utilisateurs.FindAsync(id);
            if (utilisateur != null)
            {
                _context.Utilisateurs.Remove(utilisateur);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilisateurExists(int id)
        {
          return _context.Utilisateurs.Any(e => e.IdUtilisateur == id);
        }
    }
}
