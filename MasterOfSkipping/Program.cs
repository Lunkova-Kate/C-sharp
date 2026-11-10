namespace MasterOfSkipping;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


class Program
{

    private static async Task<SimulationSession> CreateNewSessionAsync(SemesterRepo semesterRepo)
    {
        Console.Write("Enter seed: ");
        string? input = Console.ReadLine();

        if (!int.TryParse(input, out int seed))
        {
            Console.WriteLine("Seed not provided or invalid. Using random seed.");
            seed = Random.Shared.Next();
            Console.WriteLine($"Using seed: {seed}");

        }
        var random = new Random(seed);

        Professor[] professors =
            ProfessorGenerator.Generate(random);

        ISkipStrategy strategy = new MyStrategy();

        var semester = new Semester(professors, strategy);

        Guid semesterId =
            await semesterRepo.AddNewSemesterToDatabase(
                seed,
                professors);

        Console.WriteLine($"Created a new semester: {semesterId}");

        return new SimulationSession(semesterId, semester);
    }
    private static async Task<SimulationSession?> RestoreSessionAsync(SemesterRepo semesterRepo)
    {
        List<SemesterEntity> unfinishedSemesters =
            await semesterRepo.GetUnfinishedSemestersAsync();

        if (unfinishedSemesters.Count == 0)
        {
            Console.WriteLine("There are no unfinished semesters.");
            return null;
        }

        Console.WriteLine("Unfinished semesters:");

        foreach (SemesterEntity semesterInfo in unfinishedSemesters)
        {
            Console.WriteLine(
                $"ID: {semesterInfo.Id}, " +
                $"days completed: {semesterInfo.DaysCompleted}, " +
                $"enjoyment: {semesterInfo.TotalEnjoyment}");
        }

        Console.Write("Enter semester ID: ");
        string? input = Console.ReadLine();

        if (!Guid.TryParse(input, out Guid semesterId))
        {
            Console.WriteLine("Invalid semester ID.");
            return null;
        }

        bool isUnfinishedSemester = unfinishedSemesters
            .Any(semesterInfo => semesterInfo.Id == semesterId);

        if (!isUnfinishedSemester)
        {
            Console.WriteLine(
                "The selected semester does not exist or is already completed.");
            return null;
        }

        SemesterEntity? savedSemester =
            await semesterRepo.GetSemesterByIdAsync(semesterId);

        if (savedSemester is null)
        {
            Console.WriteLine("The selected semester was not found.");
            return null;
        }

        var semesterRestorer = new SemesterRestorer();
        ISkipStrategy strategy = new MyStrategy();

        Semester semester = semesterRestorer.Restore(
            savedSemester,
            strategy);

        Console.WriteLine(
            $"Semester restored. " +
            $"Days completed: {semester.DaysElapsed}, " +
            $"enjoyment: {semester.TotalEnjoyment}");


        return new SimulationSession(semesterId, semester);
    }
    static async Task Main(string[] args)
    {

        if (args.Length == 0)
        {
            Console.WriteLine("Please specify mode: new or resume.");
            return;
        }
        string mode = args[0].ToLowerInvariant();
        if (mode != "new" && mode != "resume")
        {
            Console.WriteLine(
                $"Unknown mode: '{mode}'. Please use 'new' or 'resume'.");
            return;
        }


        var builder = Host.CreateApplicationBuilder();

        string connectionString =
            builder.Configuration.GetConnectionString("MasterOfSkipping")
            ?? throw new InvalidOperationException(
                "Connection string 'MasterOfSkipping' was not found.");

        var options =
            new DbContextOptionsBuilder<AppOnDBContext>()
                .UseNpgsql(connectionString)
                .Options;

        await using var dbContext = new AppOnDBContext(options);
        var semesterRepo = new SemesterRepo(dbContext);
        SimulationSession? session = null;

        switch (mode)
        {
            case "new":
                session = await CreateNewSessionAsync(semesterRepo);
                break;
            case "resume":
                session = await RestoreSessionAsync(semesterRepo);
                break;

        }
        if (session is null)
        {
            return;
        }

        builder.Services.AddSingleton(session);
        builder.Services.AddSingleton(semesterRepo);

        builder.Services.AddHostedService<MyWorker>();
        builder.Services.AddHostedService<KeyboardStopService>();

        using IHost host = builder.Build();

        await host.RunAsync();

    }


}


// BenchmarkRunner.Run(iterations: 10000, seed: Random.Shared.Next());
// return;


// // Console.ReadKey();

// var builder = Host.CreateApplicationBuilder(args);

// string connectionSetting =
//     builder.Configuration.GetConnectionString("MasterOfSkipping")
//         ?? throw new InvalidOperationException(
//             "Connection string 'MasterOfSkipping' was not found.");

// // builder.Services.AddDbContext<AppOnDBContext>(options =>
// //     options.UseNpgsql(connectionSetting));



// // int seed = 123;
// // Random random = new Random(seed);
// // Professor[] professors = ProfessorGenerator.Generate(random);
// // MyStrategy strategy = new MyStrategy();
// // Semester semester = new Semester(professors, strategy);

// using var _db = new AppOnDBContext(
//     new DbContextOptionsBuilder<AppOnDBContext>()
//         .UseNpgsql(connectionSetting)
//         .Options);
// {

//     SemesterRepo semesterRepo = new SemesterRepo(_db);
//     List<SemesterEntity> unfinishedSemesters = await semesterRepo.GetUnfinishedSemestersAsync();


//     var lastSemester = await semesterRepo.GetSemesterByIdAsync(unfinishedSemesters.Last().Id)
//         ?? throw new InvalidOperationException("The selected semester was not found.");
//     Console.WriteLine($"DaysCount {lastSemester.SemesterDays.Count}, SUbjectCount {lastSemester.SubjectSettings.Count()}, LessonsCount {lastSemester.SemesterDays.Sum(day => day.Lessons.Count)}");
//     var semesterRestorer = new SemesterRestorer();
//     Semester restoredSemester = semesterRestorer.Restore(
//         lastSemester,
//         new MyStrategy());

//     // DayResult nextDay = restoredSemester.ProcessDay();
//     // await semesterRepo.SaveDayAsync(lastSemester.Id, nextDay);

//     Console.WriteLine(
//         $"Restored: Days={restoredSemester.DaysElapsed}, " +
//         $"Enjoyment={restoredSemester.TotalEnjoyment}");

//     Console.WriteLine(
//         $"Database: Days={lastSemester.DaysCompleted}, " +
//         $"Enjoyment={lastSemester.TotalEnjoyment}");



// }
