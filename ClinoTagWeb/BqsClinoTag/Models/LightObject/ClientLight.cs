namespace BqsClinoTag.Models.LightObject
{
    public class ClientLight
    {
        public int IdClient { get; set; }
        public string Nom { get; set; }

        public ClientLight(Client c)
        {
            IdClient = c.IdClient;
            Nom = c.Nom;
        }
    }
}
