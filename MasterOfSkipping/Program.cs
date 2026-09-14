namespace MasterOfSkipping;

class Program
{
    static void Main(string[] args)
    {
        int seed;
        if (args.Length > 0 && int.TryParse(args[0], out int value))
        {
            seed = value;
        }
        else
        {
            seed = Random.Shared.Next();
            Console.WriteLine("Seed not provided or invalid. Using random seed.");
        }

        Console.WriteLine($"Seed: {seed}");
        var random = new Random(seed);
        RunMasterOfSkipping(random);
    }

    static void RunMasterOfSkipping(Random random)
    {

        Professor[] professors = ProfessorGenerator.Generate(random);
        ISkipStrategy strategy = new AlwaysAttendStrategy();
        Semester simulator = new Semester(professors, strategy);

        int total = simulator.Run(out int days, out bool expelled);
        Console.WriteLine(expelled
            ? $"Expelled on day {days}. Total enjoyment: {total}"
            : $"Total enjoyment: {total}");
        Console.WriteLine($"Average per day: {total / (double)days:F2}");


    }


}
