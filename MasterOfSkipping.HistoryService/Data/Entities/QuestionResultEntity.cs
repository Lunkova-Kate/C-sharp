using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping.HistoryService.Data.Entities;
[PrimaryKey(nameof(SemesterId), nameof(DayNumber), nameof(SubjectCode))]
[Table("question_result")]
public class QuestionResultEntity
{
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("day_number")]
    public short DayNumber { get; set; }

    [Column("subject_code")]
    public string SubjectCode { get; set; } = string.Empty;

    [Column("was_asked")]
    public bool WasAsked { get; set; }
    
    public LessonEntity Lesson { get; set; } = null!;
}