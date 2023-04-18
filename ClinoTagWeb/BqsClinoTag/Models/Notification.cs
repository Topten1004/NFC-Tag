using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BqsClinoTag.Models
{
    [Table("NOTIFICATION")]
    public partial class Notification
    {
        [Key]
        [Column("ID_NOTIFICATION")]
        public int IdNotification { get; set; }
        [Column("ID_UTILISATION")]
        public int IdUtilisation { get; set; }
        [Column("DH_NOTIFICATION")]
        [DataType(DataType.DateTime)]
        public DateTime DhNotification { get; set; }
        [Column("TYPE_DESTINATAIRE")]
        [StringLength(10)]
        public string TypeDestinataire { get; set; } = null!;

        [ForeignKey("IdUtilisation")]
        [InverseProperty("Notifications")]
        public virtual Utilisation IdUtilisationNavigation { get; set; } = null!;
    }
}
