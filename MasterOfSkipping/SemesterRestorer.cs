namespace MasterOfSkipping;


public class SemesterRestorer
{
    public Semester Restore(SemesterEntity savedSemester, ISkipStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(savedSemester);
        ArgumentNullException.ThrowIfNull(strategy);

        var random = new Random(savedSemester.Seed);
        Professor[] professors = ProfessorGenerator.Generate(random);
        var restoredSemester = new Semester(professors, strategy);
        bool replayedExpulsion = false;

        for (int day = 0; day < savedSemester.DaysCompleted; ++day)
        {
            DayResult dayResult = restoredSemester.ProcessDay();
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
}
