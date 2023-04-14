using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClinoTag.Models
{
    [Table("CLIENT")]
    public partial class Client
    {
        public Client()
        {
            Lieus = new HashSet<Lieu>();
        }

        [Key]
        [Column("ID_CLIENT")]
        public int IdClient { get; set; }
        [Column("NOM")]
        [StringLength(100)]
        public string Nom { get; set; } = null!;

        [InverseProperty("IdClientNavigation")]
        public virtual ICollection<Lieu> Lieus { get; set; }
    }
}
