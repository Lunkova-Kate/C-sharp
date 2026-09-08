using System;

namespace MasterOfSkipping;

class Program
{
    static void Main(string[] args)
    {
        int seed = args.Length > 0 && int.TryParse(args[0], out int value)
    ? value
    : Random.Shared.Next();
        Console.WriteLine($"Seed: {seed}");
        var random = new Random(seed);
        RunMasterOfSkipping(random);
    }

    static void RunMasterOfSkipping(Random random)
    {

        Professor[] professors = ProfessorGenerator.Generate(random);
        ISkipStrategy strategy = new AlwaysAttendStrategy();
        Semestr simulator = new Semestr(professors, strategy);

        int totalEnjoyment = simulator.Run();

        Console.WriteLine($"Total enjoyment: {totalEnjoyment}");
        Console.WriteLine($"Average per day: {totalEnjoyment / 100.0:F2}");
    }


}
