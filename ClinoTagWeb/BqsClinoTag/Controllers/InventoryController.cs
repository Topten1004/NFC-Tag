using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.ViewModel.Inventory;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using static BqsClinoTag.Grool.Droits;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN) + "," + nameof(Droits.Roles.MANAGER))]

    public class InventoryController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public InventoryController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id, int? flag, string? filter)
        {

            string? userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            int idUtilisateur = 0;

            if (userRole == nameof(Droits.Roles.MANAGER))
            {
                idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }

            var lieus = await _context.Lieus.Where(x => x.Inventory == true && (filter == null || x.Nom.Contains(filter)) && (idUtilisateur == 0 || x.IdClient == idUtilisateur)).OrderBy(x => x.Nom).ToListAsync();

            var model = new InventoryVM();

            if (flag != null && flag != 0)
            {
                model.flag = flag;
            }

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

                    string base64String = Convert.ToBase64String(passages?.Photo ?? new byte[0]);

                    model.photo = base64String;
                }

                var taskIds = await _context.TacheLieus.Where(x => x.IdLieu == id).Include(c => c.PassageTaches).ToListAsync();

                foreach (var taskId in taskIds)
                {
                    var passItem = taskId.PassageTaches.Where(x => x.Fait == false);

                    if (passItem != null)
                    {
                        var temp = new TaskVM();

                        var temps = _context.Taches.Where(x => x.IdTache == taskId.IdTache).FirstOrDefault();

                        if (temps != null)
                        {
                            temp.IdTask = temps.IdTache;
                            temp.Description = temps.Description ?? new string("");

                            model.tasks.Add(temp);
                        }
                    }
                }
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


        // GET: Inventory/IsComment/5
        public async Task<IActionResult> IsComment(int? id)
        {
            return RedirectToAction(nameof(Index), new { id = id, flag = 1});
        }

        // GET: Inventory/IsCamera/5
        public async Task<IActionResult> IsCamera(int? id)
        {
            return RedirectToAction(nameof(Index), new { id = id, flag = 2 });
        }

        // GET: Inventory/IsFilter/
        public async Task<IActionResult> IsFilter(string? filter)
        {
            return RedirectToAction(nameof(Index), new { filter = filter });
        }
    }
}
