using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping;

[PrimaryKey(nameof(SemesterId), nameof(DayNumber))]
[Table("semester_day")]
public class SemesterDayEntity
{

    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("day_number")]
    public short DayNumber { get; set; }

    [Column("total_enjoyment")]
    public int TotalEnjoyment { get; set; } = 0;

    [Column("is_expelled")]
    public bool IsExpelled { get; set; } = false;

    public List<LessonEntity> Lessons { get; set; } = new List<LessonEntity>();
    
    public SemesterEntity Semester { get; set; } = null!;
}