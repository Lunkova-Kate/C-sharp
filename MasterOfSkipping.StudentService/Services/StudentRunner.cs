using MasterOfSkipping.StudentService.Clients;
using MasterOfSkipping.StudentService.Contracts.History;
using MasterOfSkipping.StudentService.Contracts.Professor;
using MasterOfSkipping.StudentService.Core;

namespace MasterOfSkipping.StudentService.Services;

public sealed class StudentRunner
{
    private const int DaysInSemester = 100;
    private const int PieEnjoyment = 1;

    private readonly HistoryClient _historyClient;
    private readonly ProfessorClient _professorClient;
    private readonly StudentStrategy _strategy;

    public StudentRunner(
        HistoryClient historyClient,
        ProfessorClient professorClient,
        StudentStrategy strategy)
    {
        _historyClient = historyClient;
        _professorClient = professorClient;
        _strategy = strategy;
    }

    public async Task RunAsync(
        int? seed,
        CancellationToken cancellationToken = default)
    {
        CreateSemesterResponse semester = await _historyClient
            .CreateSemesterAsync(seed, cancellationToken);

        int totalEnjoyment = 0;
        Console.WriteLine(
            $"Semester started: {semester.SemesterId}, seed: {semester.Seed}");

        for (int day = 1; day <= DaysInSemester; ++day)
        {
            await _historyClient.StartDayAsync(
                semester.SemesterId, day, cancellationToken);

            List<HistoryLessonResponse> history = await _historyClient
                .GetHistoryAsync(semester.SemesterId, day, cancellationToken);

            bool[] attendance = _strategy.DecideDay(day, history);
            bool isExpelled = false;

            for (int subjectIndex = 0;
                 subjectIndex < SubjectCatalog.Codes.Length;
                 ++subjectIndex)
            {
                string subjectCode = SubjectCatalog.Codes[subjectIndex];
                bool attended = attendance[subjectIndex];

                AskProfessorResponse result = await _professorClient.AskAsync(
                    subjectCode,
                    semester.SemesterId,
                    day,
                    attended,
                    cancellationToken);

                if (!string.Equals(
                        result.SubjectCode,
                        subjectCode,
                        StringComparison.Ordinal))
                {
                    throw new InvalidOperationException(
                        $"Expected {subjectCode}, but professor returned {result.SubjectCode}.");
                }

                if (!attended && result.WasAsked)
                {
                    totalEnjoyment = 0;
                    isExpelled = true;
                    Console.WriteLine(
                        $"Day {day}, {subjectCode}: skipped, asked, expelled.");
                    break;
                }

                if (!attended)
                {
                    ++totalEnjoyment;
                }
            }

            if (!isExpelled)
            {
                totalEnjoyment += PieEnjoyment;
            }

            SemesterStateResponse state = await _historyClient.CompleteDayAsync(
                semester.SemesterId,
                day,
                totalEnjoyment,
                isExpelled,
                cancellationToken);

            Console.WriteLine(
                $"Day {day}: enjoyment={state.TotalEnjoyment}, " +
                $"average={(double)state.TotalEnjoyment / state.DaysCompleted:F2}, " +
                $"expelled={state.IsExpelled}");

            if (state.IsExpelled)
            {
                Console.WriteLine($"Semester stopped on day {day}.");
                return;
            }
        }

        Console.WriteLine(
            $"Semester completed. Total enjoyment: {totalEnjoyment}, " +
            $"average: {(double)totalEnjoyment / DaysInSemester:F2}.");
    }
}
