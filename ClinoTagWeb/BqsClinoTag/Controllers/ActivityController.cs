using AutoMapper;
using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.Models.LightObject;
using BqsClinoTag.ViewModel.Activity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN) + "," + nameof(Droits.Roles.MANAGER))]

    public class ActivityController : Controller
    {
        private readonly CLINOTAGBQSContext _context;
        private readonly IMapper _mapper;

        public ActivityController(CLINOTAGBQSContext context, IMapper mapper)
        {
            _mapper = mapper;
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

            var lieus = await _context.Lieus.Where(x => x.Inventory == false && x.Qty == false && (filter == null || x.Nom.Contains(filter)) && (idUtilisateur == 0 || x.IdClient == idUtilisateur)).OrderBy( x => x.Nom).ToListAsync();

            var datas = new ActivityVM();
            
            foreach(var item in lieus)
            {
                var tempItem = new ActivityItem();
             
                tempItem.IdClient = item.IdClient;
                tempItem.IdLieu = item.IdLieu;
                tempItem.ActionType = item.ActionType;
                tempItem.Nom = item.Nom;
                tempItem.UidTag = item.UidTag;
                tempItem.Progress = item.Progress;

                tempItem.IsComment = 0;
                tempItem.IsCamera = 0; 

                var check = await _context.Passages.Where( x => x.IdLieu == tempItem.IdLieu).OrderBy(x => x.DhDebut).LastOrDefaultAsync();
                
                if(check != null )
                {
                    tempItem.PassageId = check.IdPassage;

                    if (check.Photo != null)
                        tempItem.IsCamera = 1;
                    if (check.Commentaire != null)
                        tempItem.IsComment = 1;
                }

                datas.datas.Add(tempItem);
            }

            if(flag != null && flag != 0)
            {
                datas.flag = flag;
            }

            if(id != 0 && id != null)
            {
                var lieu = await _context.Lieus.Where(x => x.IdLieu == id).FirstOrDefaultAsync();

                datas.lieu = lieu?.Nom;

                var passages = await _context.Passages.Where(x => x.IdLieu == id).OrderBy(x => x.DhDebut).ThenBy(x => x.IdPassage).LastOrDefaultAsync();

                if (passages != null)
                {
                    datas.comment = passages?.Commentaire ?? new string("");

                    string base64String = Convert.ToBase64String(passages?.Photo ?? new byte[0]);

                    datas.photo = base64String;
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

                            datas.tasks.Add(temp);
                        }
                    }
                }
            }

            return View(datas);
        }

        // GET: Activities/Asked/5
        public async Task<IActionResult> Asked(int? id)
        {
            var lieu = await _context.Lieus.FirstOrDefaultAsync( x => x.IdLieu  == id );
            
            if( lieu == null )
                return NotFound();

            if ( lieu.ActionType == 0)
            {
                lieu.ActionType = 1;
            }
                
            else if ( lieu.ActionType == 1)
            {
                lieu.ActionType = 0;
            }

            await _context.SaveChangesAsync();

            if (lieu.Progress == 2)
                return RedirectToAction(nameof(Index), new { id = id, flag = 3 });
            else
                return RedirectToAction(nameof(Index));
        }

        // GET: Activities/IsComment/5
        public async Task<IActionResult> IsComment(int? id)
        {
            return RedirectToAction(nameof(Index), new { id = id, flag = 1});
        }

        // GET: Activity/IsFilter/
        public async Task<IActionResult> IsFilter(string filter)
        {
            return RedirectToAction(nameof(Index), new { filter = filter });
        }

        // GET: Activities/IsCamera/5
        public async Task<IActionResult> IsCamera(int? id)
        {
            return RedirectToAction(nameof(Index), new { id = id, flag = 2 });
        }
    }
}