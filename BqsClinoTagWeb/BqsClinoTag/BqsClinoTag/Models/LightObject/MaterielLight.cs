namespace BqsClinoTag.Models.LightObject
{
    public class MaterielLight
    {
        public int IdMateriel { get; set; }
        public string UidTag { get; set; }
        public string? Client { get; set; }
        public string? Nom { get; set; }
        public string? Instruction { get; set; }
        public int Expiration { get; set; }

        public MaterielLight(Materiel materiel)
        {
            IdMateriel = materiel.IdMateriel;
            UidTag = materiel.UidTag;
            Client = materiel.IdClientNavigation.Nom;
            Nom = materiel.Nom;
            Instruction = materiel.Instruction;
            Expiration = materiel.Expiration;            
        }
    }
}