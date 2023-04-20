using BqsClinoTag.Models;
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

        public async Task<IActionResult> Index()
        {
            return View(await _context.Lieus.ToListAsync());
        }
    }
}
