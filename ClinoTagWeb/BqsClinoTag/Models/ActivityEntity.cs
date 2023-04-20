using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BqsClinoTag.Models
{
    [Table("ACTIVITY")]
    public partial class ActivityEntity
    {
        [Key]
        [Column("ID_ACTIVITY")]
        public int IdActivity { get; set; }
        [Column("NAME_LIEU")]
        [StringLength(100)]
        public string NamePlace { get; set; } = null!;

        [Column("ACTION_TYPE")]
        [StringLength(50)]
        public string ActionType { get; set; } = null!;

        [Column("FINISHED")]
        public bool Finished { get; set; } = true;

        [Column("START_DATE")]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        [Column("FINSHED_DATE")]
        [DataType(DataType.DateTime)]
        public DateTime FinishedDate { get; set; }
    }
}