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
        [Column("ID_ACKNOWLEDGE")]
        public int IdAcknowledge { get; set; }

        [Column("NOTIFICATION_DATE")]
        public DateTime NotificationDate { get; set; }

        [Column("LIEU")]
        public string Lieu { get; set; } = null!;

        [Column("NOTIFICATION")]
        public string Notification { get; set; } = null!;

        [Column("NAME")]
        public string? Name { get; set; } = null!;

        [Column("CONTACT")]
        public string? Contact { get; set; } = null!;

    }
}