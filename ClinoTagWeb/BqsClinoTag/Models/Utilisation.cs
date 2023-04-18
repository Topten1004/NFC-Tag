using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("UTILISATION")]
    public partial class Utilisation
    {
        public Utilisation()
        {
            Notifications = new HashSet<Notification>();
        }

        [Key]
        [Column("ID_UTILISATION")]
        public int IdUtilisation { get; set; }
        [Column("DH_DEBUT")]
        [DataType(DataType.DateTime)]
        public DateTime DhDebut { get; set; }
        [Column("DH_FIN")]
        [DataType(DataType.DateTime)]
        public DateTime? DhFin { get; set; }
        [Column("ID_MATERIEL")]
        public int IdMateriel { get; set; }
        [Column("ID_AGENT")]
        public int IdAgent { get; set; }
        [Column("COMMENTAIRE")]
        [StringLength(250)]
        public string? Commentaire { get; set; }
        [Column("CLOTURE")]
        [StringLength(10)]
        public string? Cloture { get; set; }

        [ForeignKey("IdAgent")]
        [InverseProperty("Utilisations")]
        public virtual Agent IdAgentNavigation { get; set; } = null!;
        [ForeignKey("IdMateriel")]
        [InverseProperty("Utilisations")]
        public virtual Materiel IdMaterielNavigation { get; set; } = null!;
        [InverseProperty("IdUtilisationNavigation")]
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
