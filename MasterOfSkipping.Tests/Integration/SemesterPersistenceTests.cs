using MasterOfSkipping;
using Microsoft.EntityFrameworkCore;

namespace MasterOfSkipping.Tests.Integration;

public sealed class SemesterPersistenceTests
{
    private const int Seed = 3;

    private static string ConnectionString =>
        Environment.GetEnvironmentVariable(
            "MASTER_OF_SKIPPING_TEST_CONNECTION")
        ?? "Host=localhost;Port=5433;" +
           "Database=master_of_skipping;" +
           "Username=postgres;Password=postgres";

    [Fact]
    [Trait("Category", "Integration")]
    public async Task SaveAndReload_PreservesSemesterState()
    {
        Guid semesterId = Guid.Empty;

        try
        {
            Professor[] professors =
                ProfessorGenerator.Generate(new Random(Seed));

            var originalSemester = new Semester(
                professors,
                new MyStrategy());

            var expectedDays = new List<DayResult>();

            await using (AppOnDBContext writeContext =
                         CreateDbContext())
            {
                var repository =
                    new SemesterRepo(writeContext);

                semesterId =
                    await repository.AddNewSemesterToDatabase(
                        Seed,
                        professors);

                for (int day = 0; day < 10; day++)
                {
                    DayResult result =
                        originalSemester.ProcessDay();

                    expectedDays.Add(result);

                    await repository.SaveDayAsync(
                        semesterId,
                        result);
                }
            }

            await using AppOnDBContext readContext =
                CreateDbContext();

            var readRepository =
                new SemesterRepo(readContext);

            SemesterEntity savedSemester =
                await readRepository.GetSemesterByIdAsync(
                    semesterId)
                ?? throw new InvalidOperationException(
                    "Saved semester was not found.");

            Assert.Equal(
                originalSemester.DaysElapsed,
                savedSemester.DaysCompleted);

            Assert.Equal(
                originalSemester.TotalEnjoyment,
                savedSemester.TotalEnjoyment);

            Assert.False(savedSemester.IsExpelled);

            Assert.Equal(
                Enum.GetValues<Subject>().Length,
                savedSemester.SubjectSettings.Count);

            Assert.Equal(
                expectedDays.Count,
                savedSemester.SemesterDays.Count);

            foreach (DayResult expectedDay in expectedDays)
            {
                SemesterDayEntity savedDay =
                    savedSemester.SemesterDays.Single(
                        day =>
                            day.DayNumber ==
                            expectedDay.DayNumber);

                Assert.Equal(
                    expectedDay.TotalEnjoyment,
                    savedDay.TotalEnjoyment);

                Assert.Equal(
                    expectedDay.IsExpelled,
                    savedDay.IsExpelled);

                Assert.Equal(
                    expectedDay.ProcessedLessonCount,
                    savedDay.Lessons.Count);

                for (int lessonIndex = 0;
                     lessonIndex <
                     expectedDay.ProcessedLessonCount;
                     lessonIndex++)
                {
                    string subjectCode =
                        DatabaseCodeMapper.SubjectToCode(
                            (Subject)lessonIndex);

                    LessonEntity savedLesson =
                        savedDay.Lessons.Single(
                            lesson =>
                                lesson.SubjectCode ==
                                subjectCode);

                    Assert.NotNull(
                        savedLesson.AttendanceDecision);

                    Assert.NotNull(
                        savedLesson.QuestionResult);

                    Assert.Equal(
                        expectedDay.Attendance[lessonIndex],
                        savedLesson
                            .AttendanceDecision!
                            .Attended);

                    Assert.Equal(
                        expectedDay.Asked[lessonIndex],
                        savedLesson
                            .QuestionResult!
                            .WasAsked);
                }
            }

            var restorer = new SemesterRestorer();

            Semester restoredSemester =
                restorer.Restore(
                    savedSemester,
                    new MyStrategy());

            Assert.Equal(
                originalSemester.DaysElapsed,
                restoredSemester.DaysElapsed);

            Assert.Equal(
                originalSemester.TotalEnjoyment,
                restoredSemester.TotalEnjoyment);
        }
        finally
        {
            await DeleteSemesterAsync(semesterId);
        }
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task RestoreAndContinue_MatchesUninterruptedRun()
    {
        Guid semesterId = Guid.Empty;

        try
        {
            Professor[] continuousProfessors =
                ProfessorGenerator.Generate(
                    new Random(Seed));

            var continuousSemester = new Semester(
                continuousProfessors,
                new MyStrategy());

            Professor[] interruptedProfessors =
                ProfessorGenerator.Generate(
                    new Random(Seed));

            var interruptedSemester = new Semester(
                interruptedProfessors,
                new MyStrategy());

            await using (AppOnDBContext writeContext =
                         CreateDbContext())
            {
                var repository =
                    new SemesterRepo(writeContext);

                semesterId =
                    await repository.AddNewSemesterToDatabase(
                        Seed,
                        interruptedProfessors);

                for (int day = 0; day < 10; day++)
                {
                    DayResult continuousResult =
                        continuousSemester.ProcessDay();

                    DayResult interruptedResult =
                        interruptedSemester.ProcessDay();

                    AssertDayResultsEqual(
                        continuousResult,
                        interruptedResult);

                    await repository.SaveDayAsync(
                        semesterId,
                        interruptedResult);
                }
            }

            await using (AppOnDBContext resumeContext =
                         CreateDbContext())
            {
                var repository =
                    new SemesterRepo(resumeContext);

                SemesterEntity savedSemester =
                    await repository.GetSemesterByIdAsync(
                        semesterId)
                    ?? throw new InvalidOperationException(
                        "Saved semester was not found.");

                var restorer = new SemesterRestorer();

                Semester restoredSemester =
                    restorer.Restore(
                        savedSemester,
                        new MyStrategy());

                for (int day = 0; day < 20; day++)
                {
                    DayResult continuousResult =
                        continuousSemester.ProcessDay();

                    DayResult restoredResult =
                        restoredSemester.ProcessDay();

                    AssertDayResultsEqual(
                        continuousResult,
                        restoredResult);

                    await repository.SaveDayAsync(
                        semesterId,
                        restoredResult);

                    if (restoredResult.IsExpelled)
                    {
                        break;
                    }
                }
            }

            await using AppOnDBContext checkContext =
                CreateDbContext();

            var checkRepository =
                new SemesterRepo(checkContext);

            SemesterEntity finalSavedState =
                await checkRepository.GetSemesterByIdAsync(
                    semesterId)
                ?? throw new InvalidOperationException(
                    "Continued semester was not found.");

            Assert.Equal(
                continuousSemester.DaysElapsed,
                finalSavedState.DaysCompleted);

            Assert.Equal(
                continuousSemester.TotalEnjoyment,
                finalSavedState.TotalEnjoyment);
        }
        finally
        {
            await DeleteSemesterAsync(semesterId);
        }
    }

    private static AppOnDBContext CreateDbContext()
    {
        DbContextOptions<AppOnDBContext> options =
            new DbContextOptionsBuilder<AppOnDBContext>()
                .UseNpgsql(ConnectionString)
                .Options;

        return new AppOnDBContext(options);
    }

    private static void AssertDayResultsEqual(
        DayResult expected,
        DayResult actual)
    {
        Assert.Equal(
            expected.DayNumber,
            actual.DayNumber);

        Assert.Equal(
            expected.TotalEnjoyment,
            actual.TotalEnjoyment);

        Assert.Equal(
            expected.IsExpelled,
            actual.IsExpelled);

        Assert.Equal(
            expected.ProcessedLessonCount,
            actual.ProcessedLessonCount);

        Assert.Equal(
            expected.Attendance,
            actual.Attendance);

        Assert.Equal(
            expected.Asked,
            actual.Asked);
    }

    private static async Task DeleteSemesterAsync(
        Guid semesterId)
    {
        if (semesterId == Guid.Empty)
        {
            return;
        }

        await using AppOnDBContext context =
            CreateDbContext();

        await context.Lessons
            .Where(lesson => lesson.SemesterId == semesterId)
            .ExecuteDeleteAsync();

        await context.SemesterDays
            .Where(day => day.SemesterId == semesterId)
            .ExecuteDeleteAsync();

        await context.SubjectSettings
            .Where(setting => setting.SemesterId == semesterId)
            .ExecuteDeleteAsync();

        await context.Semesters
            .Where(semester => semester.Id == semesterId)
            .ExecuteDeleteAsync();
    }
}
