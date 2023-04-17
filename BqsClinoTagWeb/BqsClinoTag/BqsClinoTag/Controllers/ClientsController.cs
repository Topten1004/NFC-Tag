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

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
    public class ClientsController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public ClientsController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Clients/SupprUClient/{IdUClient}")]
        public async Task<IActionResult> SupprUClient(int idUClient)
        {
            Uclient u = await _context.Uclients.FindAsync(idUClient);
            int idClient = u.IdClient;
            _context.Uclients.Remove(u);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Clients", new { id = idClient });
        }

        [HttpGet]
        [Route("Clients/AjoutUClient/{idUtilisateur}/{idClient}")]
        public async Task<bool> AjoutUClient(int idUtilisateur, int idClient)
        {
            try
            {
                Uclient u = new Uclient();
                u.IdClient = idClient;
                u.IdUtilisateur = idUtilisateur;
                _context.Uclients.Add(u);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
              return View(await _context.Clients.Include(c => c.Uclients).ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.IdClient == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdClient,Nom")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.Nom = client.Nom.ToUpper();
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Uclients).ThenInclude(u => u.IdUtilisateurNavigation)
                .Where(c => c.IdClient == id).FirstOrDefaultAsync();

            if (client == null) return NotFound();

            var excludedIDs = new HashSet<int>(client.Uclients.Select(p => p.IdUtilisateur));
            ViewBag.ListeUtilisateur = await _context.Utilisateurs
                .Where(u => (u.Role == "MANAGER" || u.Role == "ADMIN") && !excludedIDs.Contains(u.IdUtilisateur))
                .ToListAsync();
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdClient,Nom")] Client client)
        {
            if (id != client.IdClient)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    client.Nom = client.Nom.ToUpper();
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.IdClient))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Clients == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.IdClient == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Clients == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Clients'  is null.");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
          return _context.Clients.Any(e => e.IdClient == id);
        }
    }
}
