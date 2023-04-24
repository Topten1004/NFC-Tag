using AutoMapper;
using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.ViewModel.Activity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
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
                    if (flag.Photo != null)
                        tempItem.IsCamera = 1;
                    if (flag.Commentaire != null)
                        tempItem.IsComment = 1;
                }

                datas.datas.Add(tempItem);
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

                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return View();
        }
    }
}