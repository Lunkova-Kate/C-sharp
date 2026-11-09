namespace MasterOfSkipping;


public class SemesterRestorer
{
    public Semester Restore(SemesterEntity savedSemester, ISkipStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(savedSemester);
        ArgumentNullException.ThrowIfNull(strategy);

        var random = new Random(savedSemester.Seed);
        Professor[] professors = ProfessorGenerator.Generate(random);
        ValidateProfessorSettings(professors, savedSemester.SubjectSettings);

        var restoredSemester = new Semester(professors, strategy);
        bool replayedExpulsion = false;
        List<SemesterDayEntity> savedDays = savedSemester.SemesterDays
            .OrderBy(day => day.DayNumber)
            .ToList();

        if (savedDays.Count != savedSemester.DaysCompleted)
        {
            throw new InvalidOperationException(
                $"The semester says that {savedSemester.DaysCompleted} days were completed, " +
                $"but {savedDays.Count} saved days were loaded.");
        }

        for (int day = 0; day < savedSemester.DaysCompleted; ++day)
        {
            DayResult dayResult = restoredSemester.ProcessDay();
            ValidateDayHistory(dayResult, savedDays[day]);
            replayedExpulsion = dayResult.IsExpelled;

            if (replayedExpulsion && day + 1 < savedSemester.DaysCompleted)
            {
                throw new InvalidOperationException(
                    $"Semester replay expelled the student on day {dayResult.DayNumber}, " +
                    $"before the saved day {savedSemester.DaysCompleted}.");
            }
        }

        if (restoredSemester.DaysElapsed != savedSemester.DaysCompleted ||
            restoredSemester.TotalEnjoyment != savedSemester.TotalEnjoyment ||
            replayedExpulsion != savedSemester.IsExpelled)
        {
            throw new InvalidOperationException(
                "The replayed semester state does not match the saved state.");
        }

        return restoredSemester;
    }

    private static void ValidateProfessorSettings(
        Professor[] professors,
        List<SubjectSettingEntity> savedSettings)
    {
        if (savedSettings.Count != professors.Length)
        {
            throw new InvalidOperationException(
                $"Expected {professors.Length} saved professor settings, " +
                $"but found {savedSettings.Count}.");
        }

        foreach (Professor professor in professors)
        {
            string subjectCode = DatabaseCodeMapper.SubjectToCode(professor.Subject);
            SubjectSettingEntity? savedSetting = savedSettings
                .SingleOrDefault(setting => setting.SubjectCode == subjectCode);

            if (savedSetting is null)
            {
                throw new InvalidOperationException(
                    $"No saved professor setting was found for subject {subjectCode}.");
            }

            string expectedRuleCode = DatabaseCodeMapper.RuleToCode(professor.Rule);
            string? expectedDependencyACode = professor.DependencyA.HasValue
                ? DatabaseCodeMapper.SubjectToCode(professor.DependencyA.Value)
                : null;
            string? expectedDependencyBCode = professor.DependencyB.HasValue
                ? DatabaseCodeMapper.SubjectToCode(professor.DependencyB.Value)
                : null;

            if (savedSetting.RuleCode != expectedRuleCode ||
                savedSetting.DependencyACode != expectedDependencyACode ||
                savedSetting.DependencyBCode != expectedDependencyBCode)
            {
                throw new InvalidOperationException(
                    $"The regenerated professor setting for {subjectCode} " +
                    "does not match the saved setting.");
            }
        }
    }

    private static void ValidateDayHistory(
        DayResult replayedDay,
        SemesterDayEntity savedDay)
    {
        if (replayedDay.DayNumber != savedDay.DayNumber ||
            replayedDay.TotalEnjoyment != savedDay.TotalEnjoyment ||
            replayedDay.IsExpelled != savedDay.IsExpelled)
        {
            throw new InvalidOperationException(
                $"The replayed state of day {replayedDay.DayNumber} " +
                "does not match the saved day state.");
        }

        if (savedDay.Lessons.Count != replayedDay.ProcessedLessonCount)
        {
            throw new InvalidOperationException(
                $"Day {replayedDay.DayNumber} has {savedDay.Lessons.Count} saved lessons, " +
                $"but {replayedDay.ProcessedLessonCount} lessons were replayed.");
        }

        for (int lessonIndex = 0;
             lessonIndex < replayedDay.ProcessedLessonCount;
             ++lessonIndex)
        {
            string subjectCode = DatabaseCodeMapper.SubjectToCode((Subject)lessonIndex);
            LessonEntity? savedLesson = savedDay.Lessons
                .SingleOrDefault(lesson => lesson.SubjectCode == subjectCode);

            if (savedLesson is null)
            {
                throw new InvalidOperationException(
                    $"No saved lesson was found for {subjectCode} " +
                    $"on day {replayedDay.DayNumber}.");
            }

            if (savedLesson.AttendanceDecision is null ||
                savedLesson.QuestionResult is null)
            {
                throw new InvalidOperationException(
                    $"The saved lesson {subjectCode} on day {replayedDay.DayNumber} " +
                    "does not contain attendance or question data.");
            }

            if (savedLesson.AttendanceDecision.Attended !=
                    replayedDay.Attendance[lessonIndex] ||
                savedLesson.QuestionResult.WasAsked != replayedDay.Asked[lessonIndex])
            {
                throw new InvalidOperationException(
                    $"The replayed lesson {subjectCode} on day {replayedDay.DayNumber} " +
                    "does not match its saved attendance and question results.");
            }
        }
    }
}
