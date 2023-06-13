using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BqsClinoTag.Models
{

    [Table("SETTINGS")]
    public class SettingsModel
    {
        public SettingsModel()
        {
        }

        [Column("Language")]
        public bool Language { get; set; } = false!;

        [Column("Task")]
        public bool Task { get; set; } = false!;

        [Column("LanguageOne")]
        [StringLength(50)]
        public string LanguageOne { get; set; } = null!;

        [Column("LanguageTwo")]
        [DataType(DataType.Text)]
        public string? LanguageTwo { get; set; }

        [Column("LanguageThird")]
        [DataType(DataType.Text)]
        public string? LanguageThird { get; set; }

        [Column("EmailAPI")]
        [DataType(DataType.Text)]
        public string? EmailAPI { get; set; }

        [Column("Logo")]
        public byte[]? Logo { get; set; }

        [Column("ResetTime")]
        [DataType(DataType.Text)]
        public string? ResetTime { get; set; }

    }
}
