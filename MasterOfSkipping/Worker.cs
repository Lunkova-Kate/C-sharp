
namespace MasterOfSkipping;
using Microsoft.Extensions.Hosting;

public sealed class MyWorker : BackgroundService
{
    private readonly Semester _semester;
    private readonly IHostApplicationLifetime _applicationLifetime;

    public MyWorker(Semester semester, IHostApplicationLifetime applicationLifetime)
    {
        _semester = semester;
        _applicationLifetime = applicationLifetime;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
            var survived = _semester.ProcessDay();
            Console.WriteLine($"Day {_semester.DaysElapsed}: Total Enjoyment: {_semester.TotalEnjoyment}, Average Enjoyment: {_semester.AverageEnjoyment:F2}");
            if (!survived)
            {
                Console.WriteLine($"Student expelled after {_semester.DaysElapsed} days.");
                    _applicationLifetime.StopApplication();
                break;
            }
            
        }
        
    }
}