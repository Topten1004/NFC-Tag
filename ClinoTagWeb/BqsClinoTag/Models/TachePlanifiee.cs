using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("TACHE_PLANIFIEE")]
    public partial class TachePlanifiee
    {
        [Key]
        [Column("ID_TACHE_PLANIFIEE")]
        public int IdTachePlanifiee { get; set; }
        [Column("TACHE_PLANIFIEE_ACTIVE")]
        public bool TachePlanifieeActive { get; set; }
        [Column("ACTION_TACHE_PLANIFIEE")]
        [StringLength(20)]
        public string ActionTachePlanifiee { get; set; } = null!;
        [Column("CRONTAB")]
        [StringLength(30)]
        public string Crontab { get; set; } = null!;
        [Column("DESCRIPTION_CRONTAB")]
        [StringLength(120)]
        public string? DescriptionCrontab { get; set; }
        [Column("DH_TACHE_PLANIFIEE")]
        [DataType(DataType.DateTime)]
        public DateTime? DhTachePlanifiee { get; set; }
        [Column("DH_DERNIERE_TACHE")]
        [DataType(DataType.DateTime)]
        public DateTime? DhDerniereTache { get; set; }
        [Column("TACHE_ACCOMPLIE")]
        public bool TacheAccomplie { get; set; }
    }
}
