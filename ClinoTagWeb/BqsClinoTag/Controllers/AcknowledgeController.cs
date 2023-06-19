using AutoMapper;
using BqsClinoTag.Hubs;
using BqsClinoTag.Models;
using BqsClinoTag.Models.BqsClinoTag.Models;
using BqsClinoTag.ViewModel.Acknowledge;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization.Json;
using System.Security.Claims;

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
            var users = await _context.Utilisateurs.ToListAsync();
            var logs = await _context.AcknowledgeLogs.ToListAsync();

            var model = new List<AcknowledgeWebVM>();

            foreach(var item in logs)
            {
                var log = new AcknowledgeWebVM();
                var acknowledge = acknowledges.Where( x => x.IdAcknowledge == item.AcknowledgeId).FirstOrDefault();
                var user = users.Where( x => x.IdUtilisateur == item.UserId ).FirstOrDefault();

                if ( acknowledge != null && user != null)
                {
                    log.NotificationDate = acknowledge.NotificationDate;
                    log.AcknowledgeDate = item.AcknowledgeDate;
                    log.Lieu = acknowledge.Lieu;
                    log.Notification = acknowledge.Notification;
                    log.Name = acknowledge.Name;
                    log.Contact = acknowledge.Contact;
                    log.AcknowledgeBy = user.Nom + " " + user.Prenom;

                    TimeSpan duration = log.AcknowledgeDate - log.NotificationDate;
                    double durationInMinutes = duration.TotalMinutes;

                    log.Duration = (int)durationInMinutes;
                }

                model.Add(log);
            }

            return View(model);
        }


        public async Task<IActionResult> ConfirmAcknowledge([FromQuery]int AcknowledgeId)
        {
            var acknowledge = await _context.Acknowledges.Where(x => x.IdAcknowledge == AcknowledgeId).FirstOrDefaultAsync();
            var log = new AcknowledgeLog();

            if (acknowledge !=  null)
            {

                int idUtilisateur = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                log.AcknowledgeDate = DateTime.UtcNow;
                log.UserId = idUtilisateur;
                log.AcknowledgeId = AcknowledgeId;

                _context.AcknowledgeLogs.Add(log);

                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
