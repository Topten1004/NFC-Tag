using BqsClinoTag.Grool;
using BqsClinoTag.Hubs;
using BqsClinoTag.Models;
using BqsClinoTag.Models.LightObject;
using BqsClinoTag.ViewModel.Acknowledge;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BqsClinoTag.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinoTagController : ControllerBase
    {
        private readonly CLINOTAGBQSContext db;
        private IHubContext<NotificationHub, INotificationHub> hub;

        public ClinoTagController(CLINOTAGBQSContext context, IHubContext<NotificationHub, INotificationHub> _hub)
        {
            db = context;
            hub = _hub;
        }

        [HttpGet]
        [Route("GeolocAgent/{idConstructeur}/{idAgent}/{lati}/{longi}")]
        public async Task<bool> GeolocAgent(string idConstructeur, int idAgent, float lati, float longi)
        {
            try
            {
                GeolocAgent gl = new GeolocAgent
                {
                    DhGeoloc = DateTime.Now,
                    IdConstructeur = idConstructeur,
                    IdAgent = idAgent,
                    Lati = lati,
                    Longi = longi,
                    IpGeoloc = Network.GetClientIp(HttpContext)
                };
                db.GeolocAgents.Add(gl);
                await db.SaveChangesAsync();

                return true;
            }
            catch(Exception ex)
            {
                string erreur = ex.Message;
                return false;
            }
        }

        [HttpPost]
        [Route("AjoutMateriel")]
        public async Task<int> AjoutMateriel(LieuOuMaterielPost lieu)
        {
            Client unC = await db.Clients.FindAsync(lieu.idClient);

            if (unC != null)
            {
                Materiel newM = new Materiel();
                newM.IdClientNavigation = unC;
                newM.UidTag = lieu.uidTag;
                newM.Nom = lieu.nom;
                newM.Expiration = 60;
                db.Materiels.Add(newM);
                await db.SaveChangesAsync();
                return 0;
            }
            return -999;
        }

        [HttpPost]
        [Route("AjoutLieu")]
        public async Task<int> AjoutLieu(LieuOuMaterielPost lieu)
        {
            Client unC = await db.Clients.FindAsync(lieu.idClient);
            
            var checkLieu = await db.Lieus.Where(x => x.UidTag ==  lieu.uidTag).FirstOrDefaultAsync();

            if (checkLieu != null)
                return -999;

            if(unC != null)
            {
                Lieu newL = new Lieu();
                newL.IdClientNavigation = unC;
                newL.UidTag = lieu.uidTag;
                newL.Nom = lieu.nom;
                newL.ActionType = 0;
                newL.Progress = 0;
                newL.Satisfaction = 0;
                newL.Inventory = false;
                newL.Ask = string.Empty;
                newL.Contact = string.Empty;
                newL.PublicLink = "https://demo.clinotag.com/api/Clinotag/link?" + "location=" + '"' + lieu.nom + '"';
                newL.Count = 0;
                newL.Qty = false;

                db.Lieus.Add(newL);
                await db.SaveChangesAsync();
                return 0;
            }
            return -999;
        }

        [HttpPost]
        [Route("AjoutQTY")]
        public async Task<int> AjoutQTY(QtyPost qty)
        {

            Lieu? lieu = await db.Lieus.Where(x => x.UidTag == qty.uidTag).FirstOrDefaultAsync();

            if (lieu != null)
            {

                if (lieu.QtyDate.Date == DateTime.Today.Date)
                {
                    lieu.Count += qty.count;
                }
                else
                {
                    lieu.Count = qty.count;
                    lieu.QtyDate = DateTime.UtcNow.Date;
                }

                db.Update(lieu);
                await db.SaveChangesAsync();

                return 0;
            }

            return -999;
        }

        [HttpGet]
        [Route("ListeClient")]
        public async Task<List<ClientLight>> ListeClient()
        {
            List<Client> lClient = await db.Clients.OrderBy(c => c.Nom).ToListAsync();
            List<ClientLight> list = new List<ClientLight>();
            foreach (Client c in lClient) list.Add(new ClientLight(c));
            return list;
        }

        [HttpGet]
        [Route("Table/Lieu")]
        public async Task<List<Lieu>> GetAllLieus()
        {

            var lieus = db.Lieus.ToList();

            return lieus;
        }

        [HttpGet]
        [Route("Table/Satisfaction")]
        public async Task<List<SatisfactionLog>> GetAllSatisfactionLog()
        {

            var logs = db.SatisfactionLogs.ToList();

            return logs;
        }

        [HttpGet]
        [Route("Table/Acknowledge")]
        public async Task<List<Acknowledge>> GetAllAcknowledges()
        {

            var logs = db.Acknowledges.ToList();

            return logs;
        }

        [HttpGet]
        [Route("Table/Tache")]
        public async Task<List<Tache>> GetAllTaches()
        {

            var logs = db.Taches.ToList();

            return logs;
        }

        [HttpGet]
        [Route("Table/material")]
        public async Task<List<Materiel>> GetAllMaterial()
        {

            var logs = db.Materiels.ToList();

            return logs;
        }

        [HttpPost]
        [Route("NouvelleUtilisation")]
        public async Task<UtilisationLight?> NouvelleUtilisation(UtilisationLight utilisation)
        {
            try
            {
                Utilisation? utilisationExistante = await db.Utilisations
                    .Where(u => u.IdAgent == utilisation.idAgent && u.IdMateriel == utilisation.idMateriel && !u.DhFin.HasValue)
                    .FirstOrDefaultAsync();

                if (utilisationExistante == null)
                {
                    Utilisation newUtilisation = new Utilisation();
                    newUtilisation.IdAgent = utilisation.idAgent;
                    newUtilisation.IdMateriel = utilisation.idMateriel;
                    //newUtilisation.Commentaire = utilisation.commentaire;
                    newUtilisation.DhDebut = Format.JavaTimeStampToDateTime(utilisation.dhDebut);
                    //newUtilisation.DhFin = Format.JavaTimeStampToDateTime(utilisation.dhFin);

                    db.Utilisations.Add(newUtilisation);

                    await db.SaveChangesAsync();

                    return newUtilisationLight(newUtilisation);
                }
                else return newUtilisationLight(utilisationExistante);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private UtilisationLight newUtilisationLight(Utilisation u)
        {
            UtilisationLight newU = new UtilisationLight();
            newU.idUtilisation = u.IdUtilisation;
            newU.idAgent = u.IdAgent;
            newU.idMateriel = u.IdMateriel;
            newU.dhDebut = u.DhDebut.Ticks;
            newU.dhFin = u.DhFin.HasValue ? u.DhFin.Value.Ticks : null;
            newU.commentaire = u.Commentaire;
            return newU;
        }

        [HttpPost]
        [Route("UtilisationTerminee")]
        public async Task<int> UtilisationTerminee(UtilisationLight utilisation)
        {
            try
            {
                Utilisation u = await db.Utilisations.FindAsync(utilisation.idUtilisation);
                //u.IdAgent = utilisation.idAgent;
                //u.IdMateriel = utilisation.idMateriel;
                u.Commentaire = utilisation.commentaire;
                //u.DhDebut = Format.JavaTimeStampToDateTime(utilisation.dhDebut);
                u.DhFin = utilisation.dhFin.HasValue ? Format.JavaTimeStampToDateTime(utilisation.dhFin.Value) : null;

                await db.SaveChangesAsync();
                return 0;
            }
            catch (Exception ex)
            {
                return -999;
            }
        }

        [HttpPost]
        [Route("PassageEffectue")]
        public async Task<int> PassageEffectue(PassageLight passage)
        {
            try
            {
                Passage newPassage = new Passage();
                newPassage.IdAgent = passage.idAgent;
                newPassage.IdLieu = passage.idLieu;
                newPassage.Commentaire = passage.commentaire;
                if(passage.photo != null && passage.photo.Length > 0) newPassage.Photo = Convert.FromBase64String(passage.photo);
                newPassage.DhDebut = Format.JavaTimeStampToDateTime(passage.dhDebut);
                newPassage.DhFin = Format.JavaTimeStampToDateTime(passage.dhFin);

                foreach (TacheLight tl in passage.lTache)
                {
                    PassageTache newPT = new PassageTache();
                    newPT.IdPassageNavigation = newPassage;
                    newPT.IdTl = tl.idTacheLieu;
                    newPT.Fait = tl.fait;
                    newPassage.PassageTaches.Add(newPT);
                }

                db.Passages.Add(newPassage);

                await db.SaveChangesAsync();
                

                return 0;
            }
            catch (Exception ex)
            {
                return -999;
            }
        }

        [HttpGet]
        [Route("IdentificationTag/{uid}")]
        public async Task<string?> IdentificationTag(string uid)
        {
            Lieu? unL = await db.Lieus.Where(l => l.UidTag == uid).FirstOrDefaultAsync();
            if (unL != null)
            {
                if (unL.Qty == false)
                    return "LIEU";
                else
                    return "QTY";
            }
            else
            {
                Materiel? unM = await db.Materiels.Where(m => m.UidTag == uid).FirstOrDefaultAsync();
                if (unM != null) return "MATERIEL";
                else return null;
            }
        }

        [HttpGet]
        [Route("ScanLieu/{uidLieu}")]
        public async Task<LieuLight?> ScanLieu(string uidLieu)
        {
            Lieu? unL = await db.Lieus
                .Include(l => l.IdClientNavigation)
                .Include(l => l.TacheLieus).ThenInclude(tl => tl.IdTacheNavigation)
                .Where(l => l.UidTag == uidLieu).FirstOrDefaultAsync();

            // check NFC tag successfully, set progress
            if (unL.Progress != 2)
                ++unL.Progress;
            else
                unL.Progress = 1;

            await db.SaveChangesAsync();
            if (unL != null) return new LieuLight(unL);
            else return null;
        }

        [HttpGet]
        [Route("Notify")]
        public async Task<List<LieuLight>> ScanNotification()
        {
            List<Lieu>? unL = await db.Lieus
                .Include(l => l.IdClientNavigation)
                .Include(l => l.TacheLieus).ThenInclude(tl => tl.IdTacheNavigation)
                .Where(l => l.ActionType != 0).OrderBy(x => x.Nom).ToListAsync();

            var model = new List<LieuLight>();

            foreach( var item in unL)
            {
                var tempItem = new LieuLight(item);
                model.Add(tempItem);
            }

            if (unL != null) return model;
            else return null;
        }

        [HttpGet]
        [Route("ScanMateriel/{uidMateriel}")]
        public async Task<MaterielLight?> ScanMateriel(string uidMateriel)
        {
            Materiel? unM = await db.Materiels
                .Include(l => l.IdClientNavigation)                
                .Where(l => l.UidTag == uidMateriel).FirstOrDefaultAsync();
            if (unM != null) return new MaterielLight(unM);
            else return null;
        }

        [HttpGet]
        [Route("AgentLogin/{codeagent}")]
        public async Task<AgentLight?> AgentLogin(string codeagent)
        {
            Agent? unA = await db.Agents.Where(a => a.Code == codeagent).FirstOrDefaultAsync();
            if (unA != null) return new AgentLight(unA);
            else return null;
        }

        [HttpGet]
        [Route("Satisfaction")]

        public async Task<string> SatisfactionSurvey([FromQuery(Name ="location")]string lieuName, [FromQuery(Name = "satisfaction")]int satisfaction, [FromQuery(Name = "name")] string name, [FromQuery(Name = "contact")] string contact)
        {
            lieuName = lieuName.Substring(1, lieuName.Length - 2);
            var lieu = await db.Lieus.Where(x => x.Nom == lieuName).Include(p => p.Passages).ThenInclude(t => t.PassageTaches).FirstOrDefaultAsync();

            if (lieu != null)
            {
                if(lieu.Passages.LastOrDefault() != null)
                {
                    SatisfactionLog item = new SatisfactionLog
                    {
                        Contact = contact,
                        LieuName = lieuName,
                        Name = name,
                        Satisfaction = satisfaction,
                    };

                    db.SatisfactionLogs.Add(item);
                    lieu.Passages.LastOrDefault().Satisfaction = satisfaction;
                    
                    var satisfactionLogs = await db.SatisfactionLogs.Where( x => x.LieuName == lieuName ).ToListAsync();

                    lieu.Satisfaction = (int)satisfactionLogs.Average(p => p.Satisfaction);

                    await db.SaveChangesAsync();

                    return "Successfully set.";
                } else
                {
                    return "No cleaning yet.";
                }
            }
            else
            {
                return "Can't found location.";
            }
        }

        [HttpGet]
        [Route("LastVisit")]

        public async Task<LastVisit> LastVisit([FromQuery(Name = "uidTag")] string uidTag)
        {
            var lieu = await db.Lieus.Where(x => x.UidTag == uidTag).Include( p => p.Passages).ThenInclude(t => t.PassageTaches).FirstOrDefaultAsync();
            var checks = await db.PassageTaches.ToListAsync();

            if (lieu != null)
            {
                for(int i = checks.Count() - 1; i >= 0; i--)
                {
                    if (checks[i].Fait == true)
                    {
                        foreach(var item in lieu.Passages)
                        {
                            if(item.IdPassage == checks[i].IdPassage)
                            {
                                var client = await db.Clients.Where(x => x.IdClient == lieu.IdClient).FirstOrDefaultAsync();
                                var result = new LastVisit
                                {
                                    ClientName = client.Nom,
                                    LastVisitDate = item.DhFin
                                };

                                return result;
                            }
                        }
                    }
                }
            }
            return null;
        }

        [HttpGet]
        [Route("Notification")]
        public async Task<string> Notification([FromQuery(Name = "location")] string lieuName, [FromQuery(Name = "notification")] string notification, [FromQuery(Name = "name")] string? name, [FromQuery(Name = "contact")] string? contact)
        {
            lieuName = lieuName.Substring(1, lieuName.Length - 2);
            notification = notification.Substring(1, notification.Length - 2);

            Lieu? lieu = await db.Lieus.Where(x => x.Nom == lieuName).FirstOrDefaultAsync();
            if(lieu != null)
            {
                Acknowledge item = new Acknowledge();

                item.Lieu = lieuName;
                item.Notification = notification;
                item.Name = name;
                item.Contact = contact;
                item.NotificationDate = DateTime.UtcNow;

                db.Acknowledges.Add(item);

                await db.SaveChangesAsync();

                NotificationVM model = new NotificationVM();

                model.NotificationDate = item.NotificationDate;
                model.Notification = item.Notification;
                model.Lieu = item.Lieu;
                model.IdAcknowledge = item.IdAcknowledge;

                if (hub != null)
                {
                    await hub.Clients.All.NewNotification(model);
                }

                return "Sucessfully set notification";
            } else
            {
                return "Not found location";
            }
        }

        [HttpGet]
        [Route("link")]
        public async Task<string> AskClean([FromQuery(Name = "location")] string lieuName, [FromQuery(Name = "clean")] string? clean, [FromQuery(Name = "contact")] string? contact, [FromQuery(Name = "satisfaction")] int? satisfaction)
        {
            lieuName = lieuName.Substring(1, lieuName.Length - 2);
            Lieu? lieu = await db.Lieus.Where(x => x.Nom == lieuName).FirstOrDefaultAsync();
            if (lieu != null)
            {
                if(clean == "no")
                {
                    lieu.Progress = 0;
                    lieu.ActionType = 2;
                }
                else if(clean == "yes")
                {
                    lieu.Progress = 0;
                    lieu.ActionType = 1;
                }

                string PublicLink = "https://demo.clinotag.com/api/Clinotag/link?" + "location=" + '"' + lieuName + '"';
                if(clean != null)
                {
                    PublicLink += "&clean=" + clean;
                }

                if(contact != null)
                {
                    lieu.Contact = contact;
                    PublicLink += "&contact=" + contact;
                }

                if(satisfaction != null)
                {
                    lieu.Satisfaction = satisfaction;
                    PublicLink += "&satisfaction=" + satisfaction;
                }

                lieu.PublicLink = PublicLink;

                await db.SaveChangesAsync();
                return lieu.PublicLink;
            }
            else return "Not find lieu";
        }

        public bool IsEmailValid(string email)
        {
            // Regular expression pattern for email validation
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            // Check if the email matches the pattern
            bool isEmailValid = Regex.IsMatch(email, pattern);

            return isEmailValid;
        }

        [HttpGet]
        [Route("Test")]
        public async Task<List<Passage>> Test()
        {
            return await db.Passages
                .Include(p => p.IdAgentNavigation)
                .Include(p => p.IdLieuNavigation)
                .ToListAsync();
        }
    }
}
