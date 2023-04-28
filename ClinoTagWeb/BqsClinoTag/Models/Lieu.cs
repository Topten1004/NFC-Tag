using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("LIEU")]
    [Index("UidTag", Name = "TagUniqueLieu", IsUnique = true)]
    public partial class Lieu
    {
        public Lieu()
        {
            Passages = new HashSet<Passage>();
            TacheLieus = new HashSet<TacheLieu>();
        }

        [Key]
        [Column("ID_LIEU")]
        public int IdLieu { get; set; }
        [Column("NOM")]
        [StringLength(100)]
        public string Nom { get; set; } = null!;
        [Column("ID_CLIENT")]
        public int IdClient { get; set; }
        [Column("UID_TAG")]
        [StringLength(50)]
        public string UidTag { get; set; } = null!;

        [Column("ACTION_TYPE")]
        public int ActionType { get; set; } = 0!;

        [Column("PROGRESS")]
        public int Progress { get; set; } = 0!;

        [Column("Inventory")]
        public bool Inventory { get; set; } = false!;

        [ForeignKey("IdClient")]
        [InverseProperty("Lieus")]
        public virtual Client IdClientNavigation { get; set; } = null!;
        [InverseProperty("IdLieuNavigation")]
        public virtual ICollection<Passage> Passages { get; set; }
        [InverseProperty("IdLieuNavigation")]
        public virtual ICollection<TacheLieu> TacheLieus { get; set; }
    }
}
