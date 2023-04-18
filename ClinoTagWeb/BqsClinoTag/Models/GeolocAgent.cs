using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("GEOLOC_AGENT")]
    public partial class GeolocAgent
    {
        [Key]
        [Column("ID_GEOLOC_AGENT")]
        public int IdGeolocAgent { get; set; }
        [Column("ID_CONSTRUCTEUR")]
        [StringLength(100)]
        public string IdConstructeur { get; set; } = null!;
        [Column("ID_AGENT")]
        public int IdAgent { get; set; }
        [Column("LATI")]
        public double Lati { get; set; }
        [Column("LONGI")]
        public double Longi { get; set; }
        [Column("DH_GEOLOC")]
        [DataType(DataType.DateTime)]
        public DateTime DhGeoloc { get; set; }
        [Column("IP_GEOLOC")]
        [StringLength(30)]
        public string IpGeoloc { get; set; } = null!;

        [ForeignKey("IdAgent")]
        [InverseProperty("GeolocAgents")]
        public virtual Agent IdAgentNavigation { get; set; } = null!;
    }
}
