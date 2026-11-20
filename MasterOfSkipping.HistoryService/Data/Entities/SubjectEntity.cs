using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping.HistoryService.Data.Entities;

[Table("subject")]
public class SubjectEntity
{
    [Key]
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("lesson_order")]
    public short LessonOrder { get; set; }
}

