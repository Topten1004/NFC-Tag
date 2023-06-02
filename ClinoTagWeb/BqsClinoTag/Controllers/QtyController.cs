using AutoMapper;
using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.Models.LightObject;
using BqsClinoTag.ViewModel.Activity;
using BqsClinoTag.ViewModel.Qty;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BqsClinoTag.Controllers
{
    [Authorize(Roles = nameof(Droits.Roles.SUPERADMIN) + "," + nameof(Droits.Roles.ADMIN) + "," + nameof(Droits.Roles.MANAGER))]

    public class QtyController : Controller
    {
        private readonly CLINOTAGBQSContext _context;
        private readonly IMapper _mapper;

        public QtyController(CLINOTAGBQSContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<IActionResult> Index(DateTime beginDate)
        {
            if (beginDate == DateTime.MinValue)
            {
                if (HttpContext.Session.GetString("beginDate") == null) beginDate = DateTime.Now.AddMonths(-1);
                else beginDate = Convert.ToDateTime(HttpContext.Session.GetString("beginDate"));
            }
            HttpContext.Session.SetString("beginDate", beginDate.Date.ToString("yyyy-MM-dd"));
            ViewBag.beginDate = beginDate;

            var model = new QtyVM();

            var datas = await _context.Lieus.Where(x => x.Qty == true && (x.QtyDate >= beginDate && x.QtyDate <= DateTime.UtcNow)).OrderBy(x => x.QtyDate).ToListAsync();

            foreach( var item in datas)
            {
                var tempModel = new QtyVMList();
                tempModel.count = item.Count;
                tempModel.Created = item.QtyDate.Date;
                tempModel.Name = item.Nom;

                model.datas.Add(tempModel);
            }

            return View(model);
        }
    }
}