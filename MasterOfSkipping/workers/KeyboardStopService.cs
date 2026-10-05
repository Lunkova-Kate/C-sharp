namespace MasterOfSkipping;

using Microsoft.Extensions.Hosting;

public sealed class KeyboardStopService : BackgroundService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    public KeyboardStopService(IHostApplicationLifetime applicationLifetime)
    {
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                Console.WriteLine("Stopping the application...");
                _applicationLifetime.StopApplication();
                return;

            }
            await Task.Delay(100, stoppingToken);
        }
    }
}
