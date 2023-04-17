using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("AGENT")]
    [Index("Code", Name = "AgentCodeUnique", IsUnique = true)]
    public partial class Agent
    {
        public Agent()
        {
            GeolocAgents = new HashSet<GeolocAgent>();
            Passages = new HashSet<Passage>();
            Utilisations = new HashSet<Utilisation>();
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
        public virtual ICollection<GeolocAgent> GeolocAgents { get; set; }
        [InverseProperty("IdAgentNavigation")]
        public virtual ICollection<Passage> Passages { get; set; }
        [InverseProperty("IdAgentNavigation")]
        public virtual ICollection<Utilisation> Utilisations { get; set; }
    }
}
