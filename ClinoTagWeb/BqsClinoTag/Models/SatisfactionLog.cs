using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BqsClinoTag.Models
{

    [Table("SATISFACTION_LOG")]
    public class SatisfactionLog
    {
        public SatisfactionLog()
        {
        }

        [Key]
        [Column("ID_SATISFACTION")]
        public int IdSatisfaction { get; set; }

        [Column("LIEU_NAME")]
        [StringLength(50)]
        public string LieuName { get; set; } = null!;

        [Column("CONTACT")]
        [DataType(DataType.Text)]
        public string? Contact { get; set; }

        [Column("NAME")]
        [DataType(DataType.Text)]
        public string? Name { get; set; }

        [Column("Satisfaction")]
        public int Satisfaction { get; set; } = 0;
    }
}
