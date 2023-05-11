using BqsClinoTag.Models;
using BqsClinoTag.ViewModel.Inventory;
using MailKit;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BqsClinoTag.Controllers
{
    public class InventoryController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public InventoryController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, int? flag)
        {
            var lieus = await _context.Lieus.Where(x => x.Inventory == true).ToListAsync();

            var model = new InventoryVM();

            foreach (var item in lieus)
            {
                var tempItem = new InventoryItem();

                tempItem.IdClient = item.IdClient;
                tempItem.IdLieu = item.IdLieu;
                tempItem.ActionType = item.ActionType;
                tempItem.Nom = item.Nom;
                tempItem.UidTag = item.UidTag;
                tempItem.Progress = item.Progress;

                tempItem.IsComment = 0;
                tempItem.IsCamera = 0;

                var check = await _context.Passages.Where(x => x.IdLieu == tempItem.IdLieu).OrderBy(x => x.DhDebut).LastOrDefaultAsync();

                if (check != null)
                {
                    tempItem.PassageId = check.IdPassage;

                    if (check.Photo != null)
                        tempItem.IsCamera = 1;
                    if (check.Commentaire != null)
                        tempItem.IsComment = 1;
                }

                model.places.Add(tempItem);
            }

            if (id != 0 && id != null)
            {
                var lieu = await _context.Lieus.Where(x => x.IdLieu == id).FirstOrDefaultAsync();

                model.lieu = lieu?.Nom;

                var passages = await _context.Passages.Where(x => x.IdLieu == id).OrderBy(x => x.DhDebut).ThenBy(x => x.IdPassage).LastOrDefaultAsync();

                if (passages != null)
                {
                    model.comment = passages?.Commentaire ?? new string("");
                    model.photo = Convert.ToBase64String(passages?.Photo ?? new byte[0]);
                }

                var taskIds = await _context.TacheLieus.Where(x => x.IdLieu == id).Include(c => c.PassageTaches).ToListAsync();
                
                foreach (var taskId in taskIds)
                {
                    var temp = new TaskVM();

                    var temps = _context.Taches.Where(x => x.IdTache == taskId.IdTache).FirstOrDefault();

                    if(temps != null)
                    {
                        temp.IdTask = temps.IdTache;
                        temp.Description = temps.Description ?? new string("");

                        model.tasks.Add(temp);
                    }
                }
            }

            if (flag != null && flag != 0)
            {
                model.flag = flag;
            }    

            return View(model);
        }

        // GET: Inventory/Asked/5
        public async Task<IActionResult> Asked(int id)
        {
            var lieu = await _context.Lieus.Where(x => x.IdLieu == id).FirstOrDefaultAsync();

            if (lieu == null)
                return NotFound();

            if (lieu.Progress == 0 && lieu.ActionType == 0)
            {
                lieu.ActionType = 1;
            }
            else if (lieu.Progress == 0 && lieu.ActionType == 1)
            {
                lieu.ActionType = 0;
            }

            if (lieu.Progress != 2)
                id = 0;

            await _context.SaveChangesAsync();

            if (lieu.Progress == 2)
                return RedirectToAction(nameof(Index), new { id = id, flag = 3 });
            else
                return RedirectToAction(nameof(Index), new { id = id, flag = 0 }) ;
        }


        // GET: Activities/IsComment/5
        public async Task<IActionResult> IsComment(int? id)
        {
            return RedirectToAction(nameof(Index), new { id = id, flag = 1});
        }

        // GET: Activities/IsCamera/5
        public async Task<IActionResult> IsCamera(int? id)
        {
            return RedirectToAction(nameof(Index), new { id = id, flag = 2 });
        }
    }
}
