namespace BqsClinoTag.Models.LightObject
{
    public class UtilisationLight
    {
        public int idUtilisation { get; set; }
        public long dhDebut { get; set; }
        public long? dhFin { get; set; }
        public int idMateriel { get; set; }
        public int idAgent { get; set; }
        public string? commentaire { get; set; }
    }
}