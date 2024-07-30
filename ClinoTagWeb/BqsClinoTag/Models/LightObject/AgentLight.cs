namespace BqsClinoTag.Models.LightObject
{
    public class AgentLight
    {
        public int IdAgent { get; set; }
        public string Nom { get; set; }
        public bool TrainMode { get; set; }
        public AgentLight(Agent a)
        {
            IdAgent = a.IdAgent;
            Nom = a.Nom;
            TrainMode = a.TrainMode;
        }
    }
}
