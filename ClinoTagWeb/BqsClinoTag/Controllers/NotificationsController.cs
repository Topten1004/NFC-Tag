using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BqsClinoTag.Models;
using BqsClinoTag.Grool;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]
    public class NotificationsController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public NotificationsController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        // GET: Notifications
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
                    return View(await _context.Notifications
                        .Where(p => clientsID.Contains(p.IdUtilisationNavigation.IdMaterielNavigation.IdClient)
                                    && p.DhNotification.Date <= dtFin.Date && p.DhNotification.Date >= dtDebut.Date)
                        .Include(p => p.IdUtilisationNavigation).ThenInclude(u => u.IdAgentNavigation)
                        .Include(p => p.IdUtilisationNavigation).ThenInclude(p => p.IdMaterielNavigation).ThenInclude(l => l.IdClientNavigation)
                        .OrderByDescending(p => p.DhNotification).ToListAsync());
                default:
                    HttpContext.Session.SetInt32("idClient", idClient);
                    List<Client> lC = await _context.Clients.ToListAsync();
                    Client c = new Client { Nom = "<tous>", IdClient = 0 };
                    lC.Insert(0, c);
                    ViewData["IdClient"] = new SelectList(lC, "IdClient", "Nom", HttpContext.Session.GetInt32("idClient"));

                    return View(await _context.Notifications
                        .Where(p => (idClient == 0 || p.IdUtilisationNavigation.IdMaterielNavigation.IdClient == idClient)
                                    && p.DhNotification.Date <= dtFin.Date && p.DhNotification.Date >= dtDebut.Date)
                        .Include(p => p.IdUtilisationNavigation).ThenInclude(u => u.IdAgentNavigation)
                        .Include(p => p.IdUtilisationNavigation).ThenInclude(p => p.IdMaterielNavigation).ThenInclude(l => l.IdClientNavigation)
                        .OrderByDescending(p => p.DhNotification).ToListAsync());
            }
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.IdUtilisationNavigation)
                .FirstOrDefaultAsync(m => m.IdNotification == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // GET: Notifications/Create
        public IActionResult Create()
        {
            ViewData["IdUtilisation"] = new SelectList(_context.Utilisations, "IdUtilisation", "IdUtilisation");
            return View();
        }

        // POST: Notifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdNotification,IdUtilisation,DhNotification")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUtilisation"] = new SelectList(_context.Utilisations, "IdUtilisation", "IdUtilisation", notification.IdUtilisation);
            return View(notification);
        }

        // GET: Notifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            ViewData["IdUtilisation"] = new SelectList(_context.Utilisations, "IdUtilisation", "IdUtilisation", notification.IdUtilisation);
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdNotification,IdUtilisation,DhNotification")] Notification notification)
        {
            if (id != notification.IdNotification)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NotificationExists(notification.IdNotification))
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
            ViewData["IdUtilisation"] = new SelectList(_context.Utilisations, "IdUtilisation", "IdUtilisation", notification.IdUtilisation);
            return View(notification);
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Notifications == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.IdUtilisationNavigation)
                .FirstOrDefaultAsync(m => m.IdNotification == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Notifications == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Notifications'  is null.");
            }
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NotificationExists(int id)
        {
          return _context.Notifications.Any(e => e.IdNotification == id);
        }
    }
}
