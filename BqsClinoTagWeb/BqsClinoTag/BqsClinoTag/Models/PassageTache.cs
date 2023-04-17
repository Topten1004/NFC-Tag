using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("PASSAGE_TACHE")]
    public partial class PassageTache
    {
        [Key]
        [Column("ID_PT")]
        public int IdPt { get; set; }
        [Column("ID_PASSAGE")]
        public int IdPassage { get; set; }
        [Column("ID_TL")]
        public int IdTl { get; set; }
        [Column("FAIT")]
        public bool Fait { get; set; }

        [ForeignKey("IdPassage")]
        [InverseProperty("PassageTaches")]
        public virtual Passage IdPassageNavigation { get; set; } = null!;
        [ForeignKey("IdTl")]
        [InverseProperty("PassageTaches")]
        public virtual TacheLieu IdTlNavigation { get; set; } = null!;
    }
}
