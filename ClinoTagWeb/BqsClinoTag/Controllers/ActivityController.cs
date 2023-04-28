using AutoMapper;
using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.ViewModel.Activity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN))]

    public class ActivityController : Controller
    {
        private readonly CLINOTAGBQSContext _context;
        private readonly IMapper _mapper;

        public ActivityController(CLINOTAGBQSContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var lieus = await _context.Lieus.ToListAsync();

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

                var flag = await _context.Passages.Where( x => x.IdLieu == tempItem.IdLieu).OrderByDescending(x => x.DhDebut).LastOrDefaultAsync();
                
                if( flag != null )
                {
                    tempItem.PassageId = flag.IdPassage;

                    if (flag.Photo != null)
                        tempItem.IsCamera = 1;
                    if (flag.Commentaire != null)
                        tempItem.IsComment = 1;
                }

                datas.datas.Add(tempItem);
            }

            if(id != 0 && id != null)
            {
                var passage = await _context.Passages.Where(x => x.IdPassage == id).FirstOrDefaultAsync();

                if(passage != null)
                {
                    datas.comment = passage.Commentaire;
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

            if (lieu.Progress == 0 && lieu.ActionType == 0)
            {
                lieu.ActionType = 1;
            }
            else if (lieu.Progress == 0 && lieu.ActionType == 1)
            {
                lieu.ActionType = 0;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Activities/IsComment/5
        public async Task<IActionResult> IsComment(int? id)
        {
            var passage = await _context.Passages.FirstOrDefaultAsync(x => x.IdPassage == id);

            if (passage == null)
                return NotFound();

            return RedirectToAction(nameof(Index), new { id = id});
        }

        // GET: Activities/IsCamera/5
        public async Task<ActionResult> IsCamera(int? id)
        {
            var passage = await _context.Passages.FirstOrDefaultAsync(x => x.IdPassage == id);

            if (passage == null)
                return NotFound();

            string base64String = Convert.ToBase64String(passage.Photo);

            return Content(base64String, "text/plain");
        }
    }
}