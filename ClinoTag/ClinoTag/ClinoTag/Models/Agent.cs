using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinoTag.Models
{
    [Table("AGENT")]
    [Index("Code", Name = "AgentCodeUnique", IsUnique = true)]
    public partial class Agent
    {
        public Agent()
        {
            Passages = new HashSet<Passage>();
        }

        [Key]
        [Column("ID_AGENT")]
        public int IdAgent { get; set; }
        [Column("NOM")]
        [StringLength(100)]
        public string Nom { get; set; } = null!;
        [Column("CODE")]
        [StringLength(5)]
        [Unicode(false)]
        public string Code { get; set; } = null!;

        [InverseProperty("IdAgentNavigation")]
        public virtual ICollection<Passage> Passages { get; set; }
    }
}
