
namespace MasterOfSkipping;

using Microsoft.Extensions.Hosting;

public sealed class MyWorker : BackgroundService
{

    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly SimulationSession _session;
    private readonly SemesterRepo _semesterRepo;


    public MyWorker(
      SimulationSession session,
      SemesterRepo semesterRepo,
      IHostApplicationLifetime applicationLifetime)
    {
        _session = session;
        _semesterRepo = semesterRepo;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Semester semester = _session.Semester;

        while (!stoppingToken.IsCancellationRequested &&
               semester.DaysElapsed < SimulationSettings.DaysInSemester)
        {
            await Task.Delay(5000, stoppingToken);

            DayResult dayResult = semester.ProcessDay();

            await _semesterRepo.SaveDayAsync(_session.SemesterId,
                dayResult);

            Console.WriteLine($"Day {semester.DaysElapsed}: " +
                $"Total enjoyment: {semester.TotalEnjoyment}, " +
                $"average enjoyment: {semester.AverageEnjoyment:F2}");

            if (dayResult.IsExpelled)
            {
                Console.WriteLine(
                    $"Student expelled after {semester.DaysElapsed} days.");

                _applicationLifetime.StopApplication();
                break;
            }

            if (semester.DaysElapsed >=
                SimulationSettings.DaysInSemester)
            {
                Console.WriteLine(
                    $"Semester completed. " +
                    $"Total enjoyment: {semester.TotalEnjoyment}, " +
                    $"average: {semester.AverageEnjoyment:F2}");

                _applicationLifetime.StopApplication();
                break;
            }
        }
    }
}