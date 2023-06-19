using AutoMapper;
using BqsClinoTag.Hubs;
using BqsClinoTag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Globalization;

namespace BqsClinoTag.Controllers
{
    public class SettingsController : Controller
    {
        private readonly CLINOTAGBQSContext _context;
        private readonly IMapper _mapper;

        public SettingsController(CLINOTAGBQSContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var adminSetting = _context.AdminSettings.FirstOrDefault();
            return View(adminSetting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Language,Task,LanguageOne,LanguageTwo,LanguageThree,EmailAPI,LOGO,ResetTime")] SettingsModel setting)
        {
            var adminSetting = _context.AdminSettings.FirstOrDefault();
            if (adminSetting != null)
            {
                adminSetting = setting;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
