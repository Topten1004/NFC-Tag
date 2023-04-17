namespace BqsClinoTag.Models.LightObject
{
    public class AgentLight
    {
        public int IdAgent { get; set; }
        public string Nom { get; set; }

        public AgentLight(Agent a)
        {
            IdAgent = a.IdAgent;
            Nom = a.Nom;
        }
    }
}
