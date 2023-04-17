namespace BqsClinoTag.Models.LightObject
{
    public class TacheLight
    {
        public int idTacheLieu { get; set; }
        public int idTache { get; set; }
        public string nom { get; set; }
        public string? description { get; set; }
        public bool fait { get; set; }
    }
}