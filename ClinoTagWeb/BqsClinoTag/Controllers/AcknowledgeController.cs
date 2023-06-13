using AutoMapper;
using BqsClinoTag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Controllers
{
    public class AcknowledgeController : Controller
    {
        private readonly CLINOTAGBQSContext _context;

        public AcknowledgeController(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var acknowledges = await _context.Acknowledges.ToListAsync();

            return View(acknowledges);
        }
    }
}
