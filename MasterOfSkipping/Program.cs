namespace MasterOfSkipping;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        // Console.ReadKey();

        int seed;
        var builder = Host.CreateApplicationBuilder(args);
        if (args.Length > 0 && int.TryParse(args[0], out int value))
        {
            seed = value;

        }
        else
        {
            Console.WriteLine("Seed not provided or invalid. Using random seed.");
            seed = Random.Shared.Next();
            Console.WriteLine($"Using seed: {seed}");


        }
        
        builder.Services.AddSingleton(new Random(seed));
        builder.Services.AddSingleton<ISkipStrategy, AlwaysAttendStrategy>();
        builder.Services.AddSingleton<Semester>(injection =>
        {
            var random = injection.GetRequiredService<Random>();
            var strategy = injection.GetRequiredService<ISkipStrategy>();
            var professors = ProfessorGenerator.Generate(random);
            return new Semester(professors, strategy);
        });
        builder.Services.AddHostedService<MyWorker>();
        builder.Services.AddHostedService<KeyboardStopService>();
        IHost host = builder.Build();
        await host.RunAsync();

    }

}
