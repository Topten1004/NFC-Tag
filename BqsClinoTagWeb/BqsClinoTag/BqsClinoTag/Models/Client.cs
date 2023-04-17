using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("CLIENT")]
    public partial class Client
    {
        public Client()
        {
            Lieus = new HashSet<Lieu>();
            Materiels = new HashSet<Materiel>();
            Uclients = new HashSet<Uclient>();
        }

        [Key]
        [Column("ID_CLIENT")]
        public int IdClient { get; set; }
        [Column("NOM")]
        [StringLength(100)]
        public string Nom { get; set; } = null!;

        [InverseProperty("IdClientNavigation")]
        public virtual ICollection<Lieu> Lieus { get; set; }
        [InverseProperty("IdClientNavigation")]
        public virtual ICollection<Materiel> Materiels { get; set; }
        [InverseProperty("IdClientNavigation")]
        public virtual ICollection<Uclient> Uclients { get; set; }
    }
}
