namespace BqsClinoTag.Models
{
    public partial class Lieu
    {
        public string ClientLieu
        {
            get
            {
                if (IdClientNavigation != null)
                    return string.Format("{0} - {1}", IdClientNavigation.Nom, Nom);
                else
                    return Nom;
            }
        }
    }

}
