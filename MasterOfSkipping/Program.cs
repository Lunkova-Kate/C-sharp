using System;

namespace MasterOfSkipping
{
    class Program
    {
        static void Main(string[] args)
        {
            RunMasterOfSkipping();
        }

        static void RunMasterOfSkipping()
        {
            Professor[] professors = ProfessorGenerator.Generate();
            ISkipStrategy strategy = new AlwaysAttendStrategy();
            Semestr simulator = new Semestr(professors, strategy);

            int totalEnjoyment = simulator.Run();

            Console.WriteLine($"Total enjoyment: {totalEnjoyment}");
            Console.WriteLine($"Average per day: {totalEnjoyment / 100.0:F2}");
        }


    }
}