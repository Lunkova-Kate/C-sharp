namespace MasterOfSkipping;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

class Program
{
     static async Task Main(string[] args)
    {

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
            builder.Services.AddSingleton<Semester>(injection => {
                var random = injection.GetRequiredService<Random>();
                var strategy = injection.GetRequiredService<ISkipStrategy>();
                var professors = ProfessorGenerator.Generate(random);
                return new Semester(professors, strategy);
            });
        builder.Services.AddHostedService<MyWorker>();
        IHost host = builder.Build();
        await host.RunAsync();

        // int count = 0;
        // for (var seed = 1; seed < 10001; ++seed)
        // {
        //     // Console.WriteLine($"Seed: {seed}");
        //     var random = new Random(seed);
        //    int total = RunMasterOfSkipping(random);
        //     count += total;
        // }
        // Console.WriteLine($"Average total: {count / (double)10000:F2}");
        // // int seed;
        // // if (args.Length > 0 && int.TryParse(args[0], out int value))
        // // {
        // //     seed = value;
        // // }
        // // else
        // // {
        // //     seed = Random.Shared.Next();
        // //     Console.WriteLine("Seed not provided or invalid. Using random seed.");
        // // }


    }

    // static int RunMasterOfSkipping(Random random)
    // {

    //     Professor[] professors = ProfessorGenerator.Generate(random);
    //     ISkipStrategy strategy = new AlwaysAttendStrategy();
    //     var simulator = new Semester(professors, strategy);

    //     int total = simulator.Run(out int days, out bool expelled);
    //     Console.WriteLine(expelled
    //         ? $"Expelled on day {days}. Total enjoyment: {total}"
    //         : $"Total enjoyment: {total}");
    //     Console.WriteLine($"Average per day: {total / (double)days:F2}");

    //     return total;
    // }


}
