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
using static System.Net.WebRequestMethods;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN) + "," + nameof(Droits.Roles.MANAGER))]
    public class LieuxController : Controller
    {
        private readonly CLINOTAGBQSContext _context;
        private string PublicLink = "https://demo.clinotag.com/api/clinoTag/link?";

        public LieuxController(CLINOTAGBQSContext context)
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
        public async Task<IActionResult> Index(int idClient = 0)
        {
            var lieus = await _context.Lieus.ToListAsync();

            foreach(var item in lieus)
            {
                if(item.PublicLink.Length == 0)
                {
                    item.PublicLink = PublicLink + "location=" + '"' + item.Nom + '"';
                }
            }

            await _context.SaveChangesAsync();

            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            switch (userRole)
            {
                case nameof(Droits.Roles.MANAGER):
                    var clientsID = new HashSet<int>(_context.Uclients.Where(u => u.IdUtilisateur == idUtilisateur).Select(p => p.IdClient));
                    return View(await _context.Lieus
                        .Where(l => clientsID.Contains(l.IdClient))
                        .Include(l => l.IdClientNavigation)
                        .Include(l => l.TacheLieus)
                        .ToListAsync());
                default:
                    HttpContext.Session.SetInt32("idClient", idClient);
                    List<Client> lC = await _context.Clients.ToListAsync();
                    Client c = new Client { Nom = "<tous>", IdClient = 0 };
                    lC.Insert(0, c);
                    ViewData["IdClient"] = new SelectList(lC, "IdClient", "Nom", HttpContext.Session.GetInt32("idClient"));

                    return View(await _context.Lieus
                        .Include(l => l.IdClientNavigation)
                        .Include(l => l.TacheLieus)
                        .Where(l => idClient == 0 || l.IdClient == idClient)
                        .ToListAsync());
            }
        }

        // GET: Lieux/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Lieus == null || !await AutoriseUtilisateur((int)id))
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

        // POST: Lieux/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLieu,Nom,UidTag,IdClient,Inventory,Qty")] Lieu lieu)
        {
            if (lieu.IdClient > 0)
            {
                if (lieu.Inventory == true && lieu.Qty == true)
                {
                    lieu.Qty = false;
                }

                lieu.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lieu.Nom.ToLower());

                lieu.PublicLink = PublicLink + "location=" + '"' + lieu.Nom + '"';

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
            if (id == null || _context.Lieus == null || !await AutoriseUtilisateur((int)id)) return NotFound();

            var lieu = await _context.Lieus
                .Include(l => l.TacheLieus).ThenInclude(tl => tl.IdTacheNavigation)
                .Include(l => l.TacheLieus).ThenInclude(tl => tl.PassageTaches)
                .Where(l => l.IdLieu == id)
                .FirstOrDefaultAsync();

            if (lieu == null) return NotFound();

            var excludedIDs = new HashSet<int>(lieu.TacheLieus.Select(p => p.IdTache));
            ViewBag.ListeTache = await _context.Taches.Where(t => !excludedIDs.Contains(t.IdTache)).ToListAsync();

            chargeListeCLient();
            return View(lieu);
        }

        // POST: Lieux/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLieu,Nom,UidTag,IdClient,Inventory,Qty")] Lieu lieu)
        {
            if (id != lieu.IdLieu)
            {
                return NotFound();
            }

            var tempLieu = await _context.Lieus.Where( x => x.IdLieu == lieu.IdLieu).FirstOrDefaultAsync();

            if (tempLieu == null)
            {
                return NotFound();
            }

            if (lieu.Inventory == true && lieu.Qty == true)
            {
                lieu.Qty = false;
            }

            tempLieu.Nom = lieu.Nom;
            tempLieu.UidTag = lieu.UidTag;
            tempLieu.IdClient = lieu.IdClient;
            tempLieu.Inventory = lieu.Inventory;
            tempLieu.Qty = lieu.Qty;


            string originalUrl = tempLieu.PublicLink;
            string newWord = lieu.Nom;

            int startIndex = originalUrl.IndexOf("\"") + 1; // Find the index of the first double quote
            int endIndex = originalUrl.LastIndexOf("\""); // Find the index of the last double quote

            string originalWord = originalUrl.Substring(startIndex, endIndex - startIndex); // Extract the original word

            string updatedUrl = originalUrl.Replace(originalWord, newWord);

            tempLieu.PublicLink = updatedUrl;

            if (tempLieu.IdClient > 0 && tempLieu.UidTag != null)
            {
                try
                {
                    tempLieu.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(lieu.Nom.ToLower());
                    _context.Update(tempLieu);
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
            if (id == null || _context.Lieus == null || !await AutoriseUtilisateur((int)id))
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

        private async Task<bool> AutoriseUtilisateur(int idLieu)
        {
            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == nameof(Droits.Roles.SUPERADMIN) || userRole == nameof(Droits.Roles.ADMIN)) return true;

            Lieu? l = await _context.Lieus.FindAsync(idLieu);
            if (l == null) return false;
            int idClient = l.IdClient;

            int idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var clientsID = new HashSet<int>(_context.Uclients.Where(u => u.IdUtilisateur == idUtilisateur).Select(p => p.IdClient));
            if (clientsID.Contains(idClient)) return true;
            else return false;
        }

        // POST: Lieux/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Lieus == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Lieus'  is null.");
            }
            var lieu = await _context.Lieus.FindAsync(id);
            if (lieu != null && await AutoriseUtilisateur(id))
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