using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("UCLIENT")]
    [Index("IdUtilisateur", "IdClient", Name = "NonClusteredIndex-UtilisateurClient", IsUnique = true)]
    public partial class Uclient
    {
        [Key]
        [Column("ID_UCLIENT")]
        public int IdUclient { get; set; }
        [Column("ID_UTILISATEUR")]
        public int IdUtilisateur { get; set; }
        [Column("ID_CLIENT")]
        public int IdClient { get; set; }

        [ForeignKey("IdClient")]
        [InverseProperty("Uclients")]
        public virtual Client IdClientNavigation { get; set; } = null!;
        [ForeignKey("IdUtilisateur")]
        [InverseProperty("Uclients")]
        public virtual Utilisateur IdUtilisateurNavigation { get; set; } = null!;
    }
}
