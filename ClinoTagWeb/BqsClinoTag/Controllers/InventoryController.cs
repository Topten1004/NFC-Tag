using BqsClinoTag.Models;
using BqsClinoTag.ViewModel.Inventory;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Controllers
{
    public class InventoryController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public InventoryController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var model = new InventoryVM();

            model.places = await _context.Lieus.ToListAsync();

            if (id != 0 && id != null)
            {
                var taskIds = await _context.TacheLieus.Where(x => x.IdLieu == id).ToListAsync();
                
                foreach (var taskId in taskIds)
                {
                    var temp = new TaskVM();

                    var temps = _context.Taches.Where(x => x.IdTache == taskId.IdTache).FirstOrDefault();

                    temp.IdTask = temps.IdTache;
                    temp.Description = temps.Description;

                    model.tasks.Add(temp);
                }
            }

            return View(model);
        }

        // GET: Inventory/Asked/5
        public async Task<IActionResult> Asked(int id)
        {
            var lieu = await _context.Lieus.Where(x => x.IdLieu == id).FirstOrDefaultAsync();

            if (lieu.Progress != 2)
                id = 0;

            TempData["ID"] = id;

            return RedirectToAction(nameof(Index), new {id = id});
        }
    }
}
