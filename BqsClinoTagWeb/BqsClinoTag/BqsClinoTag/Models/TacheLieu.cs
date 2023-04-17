using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("TACHE_LIEU")]
    [Index("IdLieu", "IdTache", Name = "IX_TACHE_LIEU", IsUnique = true)]
    public partial class TacheLieu
    {
        public TacheLieu()
        {
            PassageTaches = new HashSet<PassageTache>();
        }

        [Key]
        [Column("ID_TL")]
        public int IdTl { get; set; }
        [Column("ID_TACHE")]
        public int IdTache { get; set; }
        [Column("ID_LIEU")]
        public int IdLieu { get; set; }

        [ForeignKey("IdLieu")]
        [InverseProperty("TacheLieus")]
        public virtual Lieu IdLieuNavigation { get; set; } = null!;
        [ForeignKey("IdTache")]
        [InverseProperty("TacheLieus")]
        public virtual Tache IdTacheNavigation { get; set; } = null!;
        [InverseProperty("IdTlNavigation")]
        public virtual ICollection<PassageTache> PassageTaches { get; set; }
    }
}
