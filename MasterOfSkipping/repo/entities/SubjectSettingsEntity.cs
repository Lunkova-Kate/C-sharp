using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace MasterOfSkipping;

[Table("subject_setting")]
[PrimaryKey(nameof(SemesterId), nameof(SubjectCode))]
public class SubjectSettingEntity
{
    [Column("semester_id")]
    public Guid SemesterId { get; set; }

    [Column("subject_code")]
    public string SubjectCode { get; set; } = string.Empty;

    [Column("rule_code")]
    public string RuleCode { get; set; } = string.Empty;

    [Column("dependency_a_code")]
    public string? DependencyACode { get; set; }

    [Column("dependency_b_code")]
    public string? DependencyBCode { get; set; }


    public List<LessonEntity> Lessons { get; set; } = new List<LessonEntity>();
    public SemesterEntity Semester { get; set; } = null!;
    public SubjectEntity Subject { get; set; } = null!;
    public RuleEntity Rule { get; set; } = null!;
    public SubjectEntity? DependencyA { get; set; }
    public SubjectEntity? DependencyB { get; set; }
}