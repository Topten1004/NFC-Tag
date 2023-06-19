namespace BqsClinoTag.Models
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Linq;
    using System.Security.Cryptography.X509Certificates;

    namespace BqsClinoTag.Models
    {

        [Table("acknowledge_log")]
        public class AcknowledgeLog
        {

            [Key]
            [Column("ID_ACKNOWLEDGELOG")]
            public int IdAcknowledgeLog { get; set; }

            [Column("ACKNOWLEDGE_ID")]
            public int AcknowledgeId { get; set; }

            [Column("ACKNOWLEDGE_DATE")]
            public DateTime AcknowledgeDate { get; set; }

            [Column("USER_ID")]
            public int UserId { get; set; }

        }
    }
}
