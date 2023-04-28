using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("utilisateur")]
    public partial class Utilisateur
    {
        public Utilisateur()
        {
            Uclients = new HashSet<Uclient>();
        }

        [Key]
        [Column("ID_UTILISATEUR")]
        public int IdUtilisateur { get; set; }
        [Column("NOM")]
        [StringLength(50)]
        public string Nom { get; set; } = null!;
        [Column("PRENOM")]
        [StringLength(50)]
        public string Prenom { get; set; } = null!;
        [Column("LOGIN")]
        [StringLength(50)]
        public string Login { get; set; } = null!;
        [Column("EMAIL")]
        [StringLength(100)]
        public string Email { get; set; } = null!;
        [Column("MDP")]
        [StringLength(100)]
        public string Mdp { get; set; } = null!;
        [Column("ROLE")]
        [StringLength(10)]
        public string Role { get; set; } = null!;

        [ForeignKey("Role")]
        [InverseProperty("Utilisateurs")]
        public virtual Role RoleNavigation { get; set; } = null!;
        [InverseProperty("IdUtilisateurNavigation")]
        public virtual ICollection<Uclient> Uclients { get; set; }
    }
}
