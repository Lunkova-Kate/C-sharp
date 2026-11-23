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

    public async Task RunNewAsync(
        int? seed,
        CancellationToken cancellationToken = default)
    {
        CreateSemesterResponse semester = await _historyClient
            .CreateSemesterAsync(seed, cancellationToken);

        Console.WriteLine(
            $"Semester started: {semester.SemesterId}, seed: {semester.Seed}");

        await RunSemesterAsync(
            semester.SemesterId,
            firstDay: 1,
            initialEnjoyment: 0,
            cancellationToken);
    }

    public async Task ResumeAsync(
        CancellationToken cancellationToken = default)
    {
        List<SemesterStateResponse> unfinishedSemesters = await _historyClient
            .GetUnfinishedSemestersAsync(cancellationToken);

        if (unfinishedSemesters.Count == 0)
        {
            Console.WriteLine("There are no unfinished semesters.");
            return;
        }

        Console.WriteLine("Unfinished semesters:");

        foreach (SemesterStateResponse semester in unfinishedSemesters)
        {
            Console.WriteLine(
                $"ID: {semester.SemesterId}, " +
                $"days completed: {semester.DaysCompleted}, " +
                $"enjoyment: {semester.TotalEnjoyment}");
        }

        Console.Write("Enter semester ID: ");
        string? input = Console.ReadLine();

        if (!Guid.TryParse(input, out Guid semesterId))
        {
            Console.WriteLine("Invalid semester ID.");
            return;
        }

        bool canBeResumed = unfinishedSemesters.Any(
            semester => semester.SemesterId == semesterId);

        if (!canBeResumed)
        {
            Console.WriteLine(
                "The selected semester does not exist or cannot be resumed.");
            return;
        }

        SemesterStateResponse? semesterState = await _historyClient
            .GetSemesterAsync(semesterId, cancellationToken);

        if (semesterState is null)
        {
            Console.WriteLine("The selected semester was not found.");
            return;
        }

        Console.WriteLine(
            $"Semester resumed. Days completed: {semesterState.DaysCompleted}, " +
            $"enjoyment: {semesterState.TotalEnjoyment}");

        await RunSemesterAsync(
            semesterId,
            firstDay: semesterState.DaysCompleted + 1,
            initialEnjoyment: semesterState.TotalEnjoyment,
            cancellationToken);
    }

    private async Task RunSemesterAsync(
        Guid semesterId,
        int firstDay,
        int initialEnjoyment,
        CancellationToken cancellationToken)
    {
        int totalEnjoyment = initialEnjoyment;

        for (int day = firstDay; day <= DaysInSemester; ++day)
        {
            await _historyClient.StartDayAsync(
                semesterId, day, cancellationToken);

            List<HistoryLessonResponse> history = await _historyClient
                .GetHistoryAsync(semesterId, day, cancellationToken);

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
                    semesterId,
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
                semesterId,
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
