using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BqsClinoTag.Models
{

    [Table("acknowledge")]
    public class Acknowledge
    {

        [Key]
        [Column("ID_NOTIFICATION")]
        public int IdLieu { get; set; }

        [Column("NOTIFICATION_DATE")]
        public DateTime NotificationDate { get; set; }

        [Column("ACKNOWLEDGE_DATE")]
        public DateTime AcknowledgeDate { get; set; }


        [Column("LIEU")]
        public string Lieu { get; set; } = null!;

        [Column("NOTIFICATION")]
        public string Notification { get; set; } = null!;

        [Column("Name")]
        public string? Name { get; set; } = null!;

        [Column("Contact")]
        public string? Contact { get; set; } = null!;

        [Column("Acknowledge_By")]
        public string?  AcknowledgeBy{ get; set; } = null!;

        [Column("Duration")]
        public int? Duration { get; set; }  
    }
}