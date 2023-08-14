using BqsClinoTag.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BqsClinoTag.Grool;
using Cronos;

namespace BqsClinoTag.Services
{
    public class ScheduleService : BackgroundService
    {
        //        * * * * *
        //- - - - -
        //| | | | |
        //| | | | +----- day of week(0 - 6) (Sunday=0)
        //| | | +------- month(1 - 12)
        //| | +--------- day of month(1 - 31)
        //| +----------- hour(0 - 23)
        //+------------- min(0 - 59)

        //* * * * * *
        //- - - - - -
        //| | | | | |
        //| | | | | +--- day of week(0 - 6) (Sunday=0)
        //| | | | +----- month(1 - 12)
        //| | | +------- day of month(1 - 31)
        //| | +--------- hour(0 - 23)
        //| +----------- min(0 - 59)
        //+------------- sec(0 - 59)

        //Toute les 10 secondes */10 * * * * *
        //Lundi à 9h : 0 10 * * MON
        //A 17h : 0 17 * * *
        //A la 23ème minutes : */23 * * * *

        //private CrontabSchedule _schedule;
        //private DateTime _nextRun;

        private readonly CLINOTAGBQSContext context;
        private readonly IMailService mailService;

        private readonly CronExpression _cron;

        private const string schedule = "0 0 * * *"; // every day (any day of the week, any month, any day of the month, at 12:00AM)


        public ScheduleService(IServiceScopeFactory _scopeFactory, IMailService _mailService)
        {
            context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<CLINOTAGBQSContext>();
            mailService = _mailService;
            _cron = CronExpression.Parse(schedule);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                do
                {
                    var utcNow = DateTime.UtcNow;
                    var nextUtc = _cron.GetNextOccurrence(utcNow);
                    var delaySpan = nextUtc.Value - utcNow;

                    await Task.Delay(delaySpan, stoppingToken);

                    await ChangeColor();

                    IEnumerable<TachePlanifiee> ieTP = context.TachePlanifiees.Where(tp => tp.TachePlanifieeActive == true);
                    var maintenant = DateTime.Now;
                    CrontabSchedule ctPlannif;
                    bool bTacheAccomplie = false;
                    foreach (TachePlanifiee tp in ieTP)
                    {
                        await context.Entry(tp).ReloadAsync();
                        if (tp.DhTachePlanifiee == null)
                        {
                            ctPlannif = CrontabSchedule.Parse(tp.Crontab, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
                            tp.DhTachePlanifiee = ctPlannif.GetNextOccurrence(maintenant);
                        }
                        else
                        {
                            if (maintenant > tp.DhTachePlanifiee)
                            {
                                switch (tp.ActionTachePlanifiee)
                                {
                                    case "NOTIF_MATERIEL":
                                        bTacheAccomplie = await NotifMateriel(tp);
                                        break;
                                    case "LOG":
                                        bTacheAccomplie = await EcrireLog(tp);
                                        break;
                                    case "MAIL":
                                        bTacheAccomplie = await EnvoiMail(tp);
                                        break;
                                    case "TEST_CRONTAB":
                                        bTacheAccomplie = true;
                                        break;
                                }
                                ctPlannif = CrontabSchedule.Parse(tp.Crontab, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
                                tp.TacheAccomplie = bTacheAccomplie;
                                tp.DhDerniereTache = DateTime.Now;
                                tp.DhTachePlanifiee = ctPlannif.GetNextOccurrence(DateTime.Now);
                                context.Entry(tp).State = EntityState.Modified;
                            }
                        }
                    }
                    await context.SaveChangesAsync();

                    await Task.Delay(15 * 1000, stoppingToken); //15 seconds delay
                }
                while (!stoppingToken.IsCancellationRequested);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            
        }

        private async Task<bool> ChangeColor()
        {
            try
            {
                var lieus = await context.Lieus.ToListAsync();

                foreach (var item in lieus)
                {
                    item.Progress = 0;
                    context.Update(item);
                }

                await context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> NotifMateriel(TachePlanifiee tp)
        {
            try
            {
                List<Utilisation> lUtilisationExpire = await context.Utilisations
                .Include(u => u.IdAgentNavigation)
                .Include(u => u.IdMaterielNavigation)
                .ThenInclude(m => m.IdClientNavigation)
                .ThenInclude(c => c.Uclients)
                .ThenInclude(u => u.IdUtilisateurNavigation)
                .Where(u => !u.DhFin.HasValue && u.DhDebut.AddMinutes(u.IdMaterielNavigation.Expiration) < DateTime.Now)
                .ToListAsync();

                List<Tuple<Utilisateur, Utilisation>> lUC = new List<Tuple<Utilisateur, Utilisation>>();

                foreach (Utilisation u in lUtilisationExpire)
                {
                    Notification newN = new Notification();
                    newN.TypeDestinataire = "MANAGER"; // ou ADMIN
                    newN.IdUtilisation = u.IdUtilisation;
                    newN.DhNotification = DateTime.Now;
                    context.Notifications.Add(newN);

                    foreach (Uclient uc in u.IdMaterielNavigation.IdClientNavigation.Uclients)
                        lUC.Add(new Tuple<Utilisateur, Utilisation>(uc.IdUtilisateurNavigation, u));                
                    //newN.TypeDestinataire = "ADMIN";
                    foreach(Utilisateur uManager in await context.Utilisateurs.Where(u => u.RoleNavigation.Role1 == "ADMIN").ToListAsync())
                        lUC.Add(new Tuple<Utilisateur, Utilisation>(uManager, u));

                    u.DhFin = newN.DhNotification;
                    context.Entry(u).State = EntityState.Modified;
                    context.Entry(newN).State = EntityState.Added;
                }

                List<Utilisateur> lUtilisateurMailDejaEnvoye = new List<Utilisateur>();
                foreach(Tuple<Utilisateur, Utilisation> uu in lUC)
                {
                    if (!lUtilisateurMailDejaEnvoye.Contains(uu.Item1))
                    {
                        NotificationRequest request = new NotificationRequest();
                        request.utilisateur = uu.Item1;
                        request.lUtilisation = new List<Utilisation>();

                        foreach(Tuple<Utilisateur, Utilisation> uu2 in lUC.Where(u => u.Item1 == uu.Item1))
                            request.lUtilisation.Add(uu2.Item2);
                        
                        await mailService.EnvoiEmailNotificationAsync(request);                    
                        lUtilisateurMailDejaEnvoye.Add(uu.Item1);
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }

        private async Task<bool> EcrireLog(TachePlanifiee tp)
        {
            try
            {
                string dossierActuel = Directory.GetCurrentDirectory();
                // Append new lines of text to the file
                await File.AppendAllLinesAsync(
                    Path.Combine(dossierActuel, "log.txt"), 
                    new string[] { "Tâche plannifiée de " + DateTime.Now + " (" + tp.DescriptionCrontab + ")." });

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> EnvoiMail(TachePlanifiee tp)
        {
            try
            {
                MessageRequest request = new MessageRequest();
                request.FromEmail = tp.DescriptionCrontab;
                request.Message = "Mail de test, heure serveur : " + DateTime.Now.ToShortTimeString();
                request.IpEmetteur = "localhost";
                await mailService.EmailMessage(request);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}