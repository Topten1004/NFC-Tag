using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("MATERIEL")]
    [Index("UidTag", Name = "TagUniqueObjet", IsUnique = true)]
    public partial class Materiel
    {
        public Materiel()
        {
            Utilisations = new HashSet<Utilisation>();
        }

        [Key]
        [Column("ID_MATERIEL")]
        public int IdMateriel { get; set; }
        [Column("NOM")]
        [StringLength(150)]
        public string Nom { get; set; } = null!;
        [Column("INSTRUCTION")]
        [DataType(DataType.Text)]
        public string? Instruction { get; set; }
        [Column("ID_CLIENT")]
        public int IdClient { get; set; }
        [Column("UID_TAG")]
        [StringLength(50)]
        public string UidTag { get; set; } = null!;
        [Column("EXPIRATION")]
        public int Expiration { get; set; }

        [ForeignKey("IdClient")]
        [InverseProperty("Materiels")]
        public virtual Client IdClientNavigation { get; set; } = null!;
        [InverseProperty("IdMaterielNavigation")]
        public virtual ICollection<Utilisation> Utilisations { get; set; }
    }
}
