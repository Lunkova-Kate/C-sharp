using Microsoft.EntityFrameworkCore;
namespace MasterOfSkipping;

public class SemesterRepo
{
    private readonly AppOnDBContext _dbContext;

    public SemesterRepo(AppOnDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<SubjectSettingEntity> CreateSubjectSettings(Guid semesterId, Professor[] professors)
    {
        var subjectSettings =  professors.Select( professor => new SubjectSettingEntity
        {
            SemesterId = semesterId,
            SubjectCode = DatabaseCodeMapper.SubjectToCode(professor.Subject),
            RuleCode = DatabaseCodeMapper.RuleToCode(professor.Rule),
            DependencyACode = professor.DependencyA.HasValue ? DatabaseCodeMapper.SubjectToCode(professor.DependencyA.Value) : null,
            DependencyBCode = professor.DependencyB.HasValue ? DatabaseCodeMapper.SubjectToCode(professor.DependencyB.Value) : null
        }).ToList();
        
        return subjectSettings;
    }
// semester (
//         id uuid PRIMARY KEY,
//         seed integer NOT NULL,
//         days_completed smallint NOT NULL DEFAULT 0,
//         total_enjoyment integer NOT NULL DEFAULT 0,
//         is_expelled boolean NOT NULL DEFAULT false,
//         CONSTRAINT ck_semester_days_completed CHECK (days_completed BETWEEN 0 AND 100),
//         CONSTRAINT ck_semester_total_enjoyment CHECK (total_enjoyment >= 0),
//         CONSTRAINT ck_expelled_semester_enjoyment CHECK (
//             NOT is_expelled
//             OR total_enjoyment = 0
//         )
    public SemesterEntity CreateNewSemesterEntity(Guid semesterId, int seed )
    {
        var semesterEntity = new SemesterEntity
        {
            Id = semesterId,
            Seed = seed,
            DaysCompleted = 0,
            TotalEnjoyment = 0,
            IsExpelled = false
        };

        return semesterEntity;
    }

    public async Task<Guid> AddNewSemesterToDatabase(int seed, Professor[] professors)
    {
        Guid semesterId = Guid.NewGuid();
        var semesterEntity = CreateNewSemesterEntity(semesterId, seed);
        _dbContext.Semesters.Add(semesterEntity);

        var subjectSettings = CreateSubjectSettings(semesterId, professors);
        _dbContext.SubjectSettings.AddRange(subjectSettings);

        await _dbContext.SaveChangesAsync();

        return semesterId;
    }


    SemesterDayEntity CreateSemesterDayEntity(Guid semesterId, DayResult dayResult)
    {
       var semesterDayEntity = new SemesterDayEntity
        {
            SemesterId = semesterId,
            TotalEnjoyment = dayResult.TotalEnjoyment,
            DayNumber = (short)dayResult.DayNumber,
            IsExpelled = dayResult.IsExpelled
        };

        return semesterDayEntity;
    }

    List<LessonEntity> CreateLessonEntities(Guid semesterId, DayResult dayResult)
    {
        // var lessonEntities = new List<LessonEntity>();

        // for ( int i =0 ; i< dayResult.ProcessedLessonCount; ++i)
        // {
        //     var lessonEntity = new LessonEntity
        //     {
        //         SemesterId = semesterId,
        //         DayNumber = (short)dayResult.DayNumber,
        //         SubjectCode = DatabaseCodeMapper.SubjectToCode((Subject)i)
        //     };

        //     lessonEntities.Add(lessonEntity);
        // }
        var lessonEntities = Enumerable.Range(0, dayResult.ProcessedLessonCount)
        .Select(lessonIndex => new LessonEntity
        {
                SemesterId = semesterId,
                DayNumber = (short)dayResult.DayNumber,
                SubjectCode = DatabaseCodeMapper.SubjectToCode((Subject)lessonIndex)
            }).ToList();
  


        return lessonEntities;
    }

    List<AttendDecisionEntity> CreateAttendanceEntities(Guid semesterId, DayResult dayResult)
    {
        var attendanceEntities = Enumerable.Range(0, dayResult.ProcessedLessonCount)
        .Select(lessonIndex => new AttendDecisionEntity
        {
            SemesterId = semesterId,
            DayNumber = (short)dayResult.DayNumber,
            SubjectCode = DatabaseCodeMapper.SubjectToCode((Subject)lessonIndex),
            Attended = dayResult.Attendance[lessonIndex]
        }).ToList();

        return attendanceEntities;
    }


    List<QuestionResultEntity> CreateQuestionResultEntities(Guid semesterId, DayResult dayResult)
    {
        var questionResultEntities = Enumerable.Range(0, dayResult.ProcessedLessonCount)
        .Select(lessonIndex => new QuestionResultEntity
        {
            SemesterId = semesterId,
            DayNumber = (short)dayResult.DayNumber,
            SubjectCode = DatabaseCodeMapper.SubjectToCode((Subject)lessonIndex),
            WasAsked = dayResult.Asked[lessonIndex]
        }).ToList();

        return questionResultEntities;
    }

    async Task<SemesterEntity?> FindSemesterByIdAsync(Guid semesterId)
    {
        var needSemester = await _dbContext
        .Semesters.SingleOrDefaultAsync(s => s.Id == semesterId);

        return needSemester;
    }

    void UpdateSemesterProgress(SemesterEntity semesterEntity, DayResult dayResult)
    {
        semesterEntity.DaysCompleted = (short)dayResult.DayNumber;
        semesterEntity.TotalEnjoyment = dayResult.TotalEnjoyment;
        semesterEntity.IsExpelled = dayResult.IsExpelled;
    }

    public async Task SaveDayAsync (Guid semesterId, DayResult dayResult)
    {
        var currentSemester = await FindSemesterByIdAsync(semesterId);
        if (currentSemester == null)
        {
            throw new InvalidOperationException($"Semester with ID {semesterId} not found.");
        }
        UpdateSemesterProgress(currentSemester, dayResult);
        SemesterDayEntity semesterDayEntity = CreateSemesterDayEntity(semesterId, dayResult);
        List<LessonEntity> lessonEntities = CreateLessonEntities(semesterId, dayResult);
        List<AttendDecisionEntity> attendanceEntities = CreateAttendanceEntities(semesterId, dayResult);
        List<QuestionResultEntity> questionResultEntities = CreateQuestionResultEntities(semesterId, dayResult);
        _dbContext.SemesterDays.Add(semesterDayEntity);
        _dbContext.Lessons.AddRange(lessonEntities);
        _dbContext.AttendDecision.AddRange(attendanceEntities);
        _dbContext.QuestionResults.AddRange(questionResultEntities);


        await _dbContext.SaveChangesAsync();
    }

}