using Microsoft.EntityFrameworkCore;

namespace MasterOfSkipping;

public class AppOnDBContext : DbContext
{
    public AppOnDBContext(DbContextOptions<AppOnDBContext> options) : base(options)
    {
    }

    public DbSet<SubjectEntity> Subjects => Set<SubjectEntity>();
    public DbSet<RuleEntity> Rules => Set<RuleEntity>();
    public DbSet<SemesterEntity> Semesters => Set<SemesterEntity>();
    public DbSet<SubjectSettingEntity> SubjectSettings => Set<SubjectSettingEntity>();
    public DbSet<SemesterDayEntity> SemesterDays => Set<SemesterDayEntity>();
    public DbSet<LessonEntity> Lessons => Set<LessonEntity>();
    public DbSet<QuestionResultEntity> QuestionResults => Set<QuestionResultEntity>();
    public DbSet<AttendDecisionEntity> AttendDecision => Set<AttendDecisionEntity>();
}