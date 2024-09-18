using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BqsClinoTag.Models
{
    [Table("chat_history")]
    public class ChatHistory
    {
        [Key]
        [Column("id")]
        public int id { get; set; }

        [Column("room_name")]
        [StringLength(100)]

        public string RoomName { get; set; } = string.Empty;

        [Column("content")]
        [StringLength(100)]

        public string Content { get; set; } = string.Empty;

        [Column("timestamp")]
        [StringLength(100)]

        public DateTime CreatedTime { get; set; }

        [Column("checked")]

        public Boolean Checked { get; set; } = false;

        [StringLength(100)]
        [Column("language")]
        public string Language { get; set; }

        [StringLength(100)]
        [Column("user_name")]
        public string UserName { get; set; }

    }
}
