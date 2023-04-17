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
    public class AgentsController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public AgentsController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        // GET: Agents
        public async Task<bool> CodeOk(string codeSaisie, int? idAgent)
        {
            try
            {
                if (codeSaisie == null) return false;
                if (codeSaisie.Trim().Length != 5) return false;
                int codeSaisieInt = -1;
                if (Int32.TryParse(codeSaisie, out codeSaisieInt) == false) return false;
                if(idAgent.HasValue)
                    return !await _context.Agents.AnyAsync(a => a.Code == codeSaisie.Trim() && a.IdAgent != idAgent.Value);
                else
                    return !await _context.Agents.AnyAsync(a => a.Code == codeSaisie.Trim());
            }
            catch(Exception ex)
            {
                return false;
            }            
        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
              return View(await _context.Agents.ToListAsync());
        }

        // GET: Agents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Agents == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents
                .FirstOrDefaultAsync(m => m.IdAgent == id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

        // GET: Agents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAgent,Nom,Code")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                agent.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(agent.Nom.ToLower());
                _context.Add(agent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(agent);
        }

        // GET: Agents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Agents == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }
            return View(agent);
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAgent,Nom,Code")] Agent agent)
        {
            if (id != agent.IdAgent)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    agent.Nom = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(agent.Nom.ToLower());
                    _context.Update(agent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgentExists(agent.IdAgent))
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
            return View(agent);
        }

        // GET: Agents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Agents == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents
                .FirstOrDefaultAsync(m => m.IdAgent == id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Agents == null)
            {
                return Problem("Entity set 'CLINOTAGBQSContext.Agents'  is null.");
            }
            var agent = await _context.Agents.FindAsync(id);
            if (agent != null)
            {
                _context.Agents.Remove(agent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgentExists(int id)
        {
          return _context.Agents.Any(e => e.IdAgent == id);
        }
    }
}
