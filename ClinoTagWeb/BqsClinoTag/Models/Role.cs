using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("ROLE")]
    public partial class Role
    {
        public Role()
        {
            Utilisateurs = new HashSet<Utilisateur>();
        }

        [Key]
        [Column("ROLE")]
        [StringLength(10)]
        public string Role1 { get; set; } = null!;

        [InverseProperty("RoleNavigation")]
        public virtual ICollection<Utilisateur> Utilisateurs { get; set; }
    }
}
