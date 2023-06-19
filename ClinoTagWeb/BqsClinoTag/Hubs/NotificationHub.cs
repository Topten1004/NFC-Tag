using AutoMapper;
using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.Models.LightObject;
using BqsClinoTag.ViewModel.Acknowledge;
using BqsClinoTag.ViewModel.Activity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BqsClinoTag.Hubs
{
    public class NotificationHub : Hub<INotificationHub>
    {
        private readonly CLINOTAGBQSContext _context;

        public NotificationHub(CLINOTAGBQSContext context)
        {
            _context = context;
        }

        public async Task NewNotification(NotificationVM model)
        {
            await Clients.All.NewNotification(model);
        }

        public async Task SendNotification()
        {
            string idUtilisateur = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(idUtilisateur != null)
            {
                // Convert the ID to an integer if needed
                int idUtilisateurInt = Convert.ToInt32(idUtilisateur);
                int count = 0;

                var notifications = await _context.Acknowledges.ToListAsync();

                var model = new AcknowledgeVM();

                foreach (var item in notifications)
                {
                    var check = await _context.AcknowledgeLogs.Where(x => x.UserId == idUtilisateurInt && x.AcknowledgeId == item.IdAcknowledge).ToListAsync();

                    if (check.Count == 0)
                    {
                        count++;

                        var tempItem = new NotificationVM
                        {
                            IdAcknowledge = item.IdAcknowledge,
                            Lieu = item.Lieu,
                            Notification = item.Notification,
                            NotificationDate = item.NotificationDate,
                        };

                        model.Notifications.Add(tempItem);
                    }
                }

                model.NotificationCount = count;

                await Clients.All.SendNotification(model);
            }
        }

        public override async Task<Task> OnConnectedAsync()
        {
            await SendNotification();
            return base.OnConnectedAsync();
        }
    }
}
