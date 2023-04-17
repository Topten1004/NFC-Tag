namespace BqsClinoTag.Models.LightObject
{
    public class PassageLight
    {
        public int idPassage { get; set; }
        public long dhDebut { get; set; }
        public long dhFin { get; set; }
        public int idLieu { get; set; }
        public int idAgent { get; set; }
        public string commentaire { get; set; }
        public string photo { get; set; }
        public List<TacheLight> lTache { get; set; }
    }
}