using BqsClinoTag.Grool;
using BqsClinoTag.Models;
using BqsClinoTag.Models.LightObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinoTagController : ControllerBase
    {
        private readonly CLINOTAGBQSContext db;

        public ClinoTagController(CLINOTAGBQSContext context)
        {
            db = context;
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

            if(unC != null)
            {
                Lieu newL = new Lieu();
                newL.IdClientNavigation = unC;
                newL.UidTag = lieu.uidTag;
                newL.Nom = lieu.nom;
                db.Lieus.Add(newL);
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
            if (unL != null) return "LIEU";
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
            if (unL != null) return new LieuLight(unL);
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
        public async Task<AgentLight> AgentLogin(string codeagent)
        {
            Agent? unA = await db.Agents.Where(a => a.Code == codeagent).FirstOrDefaultAsync();
            if (unA != null) return new AgentLight(unA);
            else return null;
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
