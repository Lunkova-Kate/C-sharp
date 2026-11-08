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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AttendDecisionEntity>()
            .HasOne(attendance => attendance.Lesson)
            .WithOne(lesson => lesson.AttendanceDecision)
            .HasForeignKey<AttendDecisionEntity>(attendance => new
            {
                attendance.SemesterId,
                attendance.DayNumber,
                attendance.SubjectCode
            })
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionResultEntity>()
            .HasOne(questionResult => questionResult.Lesson)
            .WithOne(lesson => lesson.QuestionResult)
            .HasForeignKey<QuestionResultEntity>(questionResult => new
            {
                questionResult.SemesterId,
                questionResult.DayNumber,
                questionResult.SubjectCode
            })
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LessonEntity>()
            .HasOne(lesson => lesson.SemesterDay)
            .WithMany(semesterDay => semesterDay.Lessons)
            .HasForeignKey(lesson => new
            {
                lesson.SemesterId,
                lesson.DayNumber
            })
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LessonEntity>()
            .HasOne(lesson => lesson.SubjectSetting)
            .WithMany(subjectSetting => subjectSetting.Lessons)
            .HasForeignKey(lesson => new
            {
                lesson.SemesterId,
                lesson.SubjectCode
            })
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SemesterDayEntity>()
            .HasOne(semesterDay => semesterDay.Semester)
            .WithMany(semester => semester.SemesterDays)
            .HasForeignKey(semesterDay => semesterDay.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubjectSettingEntity>()
            .HasOne(subjectSetting => subjectSetting.Semester)
            .WithMany(semester => semester.SubjectSettings)
            .HasForeignKey(subjectSetting => subjectSetting.SemesterId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SubjectSettingEntity>()
            .HasOne(subjectSetting => subjectSetting.Subject)
            .WithMany()
            .HasForeignKey(subjectSetting => subjectSetting.SubjectCode)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SubjectSettingEntity>()
            .HasOne(subjectSetting => subjectSetting.Rule)
            .WithMany()
            .HasForeignKey(subjectSetting => subjectSetting.RuleCode)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SubjectSettingEntity>()
            .HasOne(subjectSetting => subjectSetting.DependencyA)
            .WithMany()
            .HasForeignKey(subjectSetting => subjectSetting.DependencyACode)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<SubjectSettingEntity>()
            .HasOne(subjectSetting => subjectSetting.DependencyB)
            .WithMany()
            .HasForeignKey(subjectSetting => subjectSetting.DependencyBCode)
            .OnDelete(DeleteBehavior.NoAction);
    }
}