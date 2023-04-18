using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("TACHE")]
    public partial class Tache
    {
        public Tache()
        {
            TacheLieus = new HashSet<TacheLieu>();
        }

        [Key]
        [Column("ID_TACHE")]
        public int IdTache { get; set; }
        [Column("NOM")]
        [StringLength(50)]
        public string Nom { get; set; } = null!;
        [Column("DESCRIPTION")]
        [DataType(DataType.Text)]
        public string? Description { get; set; }

        [InverseProperty("IdTacheNavigation")]
        public virtual ICollection<TacheLieu> TacheLieus { get; set; }
    }
}
