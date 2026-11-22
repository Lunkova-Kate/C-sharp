using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping.HistoryService.Data.Entities;

[PrimaryKey(nameof(SemesterId), nameof(DayNumber), nameof(SubjectCode))]
[Table("lesson")]
public class LessonEntity
{
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("day_number")]
    public short DayNumber { get; set; }

    [Column("subject_code")]
    public string SubjectCode { get; set; } = string.Empty;

    public AttendDecisionEntity? AttendanceDecision { get; set; }
    public QuestionResultEntity? QuestionResult { get; set; }

    public SemesterDayEntity SemesterDay { get; set; } = null!;

    public SubjectSettingEntity SubjectSetting { get; set; } = null!;
}
