using AutoMapper;
using BqsClinoTag.Grool;
using BqsClinoTag.Models;
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
            return View(await _context.Lieus.ToListAsync());
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