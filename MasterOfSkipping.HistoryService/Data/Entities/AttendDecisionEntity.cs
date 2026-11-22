using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping.HistoryService.Data.Entities;

[PrimaryKey(nameof(SemesterId), nameof(DayNumber), nameof(SubjectCode))]
[Table("attendance_decision")]
public class AttendDecisionEntity
{
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("day_number")]
    public short DayNumber { get; set; }

    [Column("subject_code")]
    public string SubjectCode { get; set; } = string.Empty;

    [Column("attended")]
    public bool Attended { get; set; }

    public LessonEntity Lesson { get; set; } = null!;
}
