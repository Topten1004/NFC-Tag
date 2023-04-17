using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("PASSAGE")]
    public partial class Passage
    {
        public Passage()
        {
            PassageTaches = new HashSet<PassageTache>();
        }

        [Key]
        [Column("ID_PASSAGE")]
        public int IdPassage { get; set; }
        [Column("ID_LIEU")]
        public int IdLieu { get; set; }
        [Column("ID_AGENT")]
        public int IdAgent { get; set; }
        [Column("DH_DEBUT", TypeName = "datetime")]
        public DateTime DhDebut { get; set; }
        [Column("DH_FIN", TypeName = "datetime")]
        public DateTime DhFin { get; set; }
        [Column("PHOTO")]
        public byte[]? Photo { get; set; }
        [Column("COMMENTAIRE", TypeName = "ntext")]
        public string? Commentaire { get; set; }

        [ForeignKey("IdAgent")]
        [InverseProperty("Passages")]
        public virtual Agent IdAgentNavigation { get; set; } = null!;
        [ForeignKey("IdLieu")]
        [InverseProperty("Passages")]
        public virtual Lieu IdLieuNavigation { get; set; } = null!;
        [InverseProperty("IdPassageNavigation")]
        public virtual ICollection<PassageTache> PassageTaches { get; set; }
    }
}
