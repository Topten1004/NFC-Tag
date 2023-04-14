using ClinoTag.Grool;
using ClinoTag.Models;
using ClinoTag.Models.LightObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinoTag.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClinoTagController : ControllerBase
    {
        private readonly CLINOTAGContext db;

        public ClinoTagController(CLINOTAGContext context)
        {
            db = context;
        }

        [HttpPost]
        [Route("AjoutLieu")]
        public async Task<int> AjoutLieu(LieuPost lieu)
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
        [Route("ScanLieu/{uidLieu}")]
        public async Task<LieuLight> ScanLieu(string uidLieu)
        {
            Lieu? unL = await db.Lieus
                .Include(l => l.IdClientNavigation)
                .Include(l => l.TacheLieus).ThenInclude(tl => tl.IdTacheNavigation)
                .Where(l => l.UidTag == uidLieu).FirstOrDefaultAsync();
            if (unL != null) return new LieuLight(unL);
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
