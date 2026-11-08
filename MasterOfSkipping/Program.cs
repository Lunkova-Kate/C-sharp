namespace MasterOfSkipping;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {


        // BenchmarkRunner.Run(iterations: 10000, seed: Random.Shared.Next());
        // return;


        // Console.ReadKey();

        var builder = Host.CreateApplicationBuilder(args);

        string connectionSetting =
            builder.Configuration.GetConnectionString("MasterOfSkipping")
                ?? throw new InvalidOperationException(
                    "Connection string 'MasterOfSkipping' was not found.");

        // builder.Services.AddDbContext<AppOnDBContext>(options =>
        //     options.UseNpgsql(connectionSetting));



        int seed = 123;
        Random random = new Random(seed);
        Professor[] professors = ProfessorGenerator.Generate(random);
        MyStrategy strategy = new MyStrategy();
        Semester semester = new Semester(professors, strategy);

        using var _db = new AppOnDBContext(
            new DbContextOptionsBuilder<AppOnDBContext>()
                .UseNpgsql(connectionSetting)
                .Options);
        {

            SemesterRepo semesterRepo = new SemesterRepo(_db);
            List<SemesterEntity> unfinishedSemesters = await semesterRepo.GetUnfinishedSemestersAsync();
            // foreach (var i in unfinishedSemesters)
            // {
            //   Console.WriteLine($"Unfinished semester: Id: {i.Id}, Seed: {i.Seed}, DaysCompleted: {i.DaysCompleted}, TotalEnjoyment: {i.TotalEnjoyment}, IsExpelled: {i.IsExpelled}");
            // }

            var lastSemester = await semesterRepo.GetSemesterByIdAsync(unfinishedSemesters.Last().Id);
            // Console.WriteLine($"Last unfinished semester: Id: {lastSemester.Id} and  {lastSemester.SubjectSettings.Count}");
            Console.WriteLine($"DaysCount {lastSemester.SemesterDays.Count}, SUbjectCount {lastSemester.SubjectSettings.Count()}, LessonsCount {lastSemester.SemesterDays.Sum(day => day.Lessons.Count)}");    
                  foreach (var day in lastSemester.SemesterDays.OrderBy(day => day.DayNumber))
{
    Console.WriteLine(
        $"Day {day.DayNumber}: " +
        $"Enjoyment={day.TotalEnjoyment}, " +
        $"Expelled={day.IsExpelled}");

    foreach (var lesson in day.Lessons.OrderBy(lesson => lesson.SubjectCode))
    {
        string attended =
            lesson.AttendanceDecision?.Attended.ToString() ?? "missing";

        string wasAsked =
            lesson.QuestionResult?.WasAsked.ToString() ?? "missing";

        Console.WriteLine(
            $"  {lesson.SubjectCode}: " +
            $"Attended={attended}, " +
            $"WasAsked={wasAsked}");
    }
}
             // var semesterId = await semesterRepo.AddNewSemesterToDatabase(seed, professors);
            // for (int i = 0; i < 5; ++i)
            // {
            //     DayResult dayResult = semester.ProcessDay();
            //     await semesterRepo.SaveDayAsync(semesterId, dayResult);
            //     Console.WriteLine($"Id semester: {semesterId}, Day: {dayResult.DayNumber}, Enjoyment: {dayResult.TotalEnjoyment}, Expelled: {dayResult.IsExpelled}");
            // }
            
         
            
        }



        // builder.Services.AddSingleton(new Random(seed));
        // builder.Services.AddSingleton<ISkipStrategy, MyStrategy>();
        // builder.Services.AddSingleton<Semester>(injection =>
        // {
        //     var random = injection.GetRequiredService<Random>();
        //     var strategy = injection.GetRequiredService<ISkipStrategy>();
        //     var professors = ProfessorGenerator.Generate(random);
        //     return new Semester(professors, strategy);
        // });

        // int seed;
        // var builder = Host.CreateApplicationBuilder(args);
        // if (args.Length > 0 && int.TryParse(args[0], out int value))
        // {
        //     seed = value;

        // }
        // else
        // {
        //     Console.WriteLine("Seed not provided or invalid. Using random seed.");
        //     seed = Random.Shared.Next();
        //     Console.WriteLine($"Using seed: {seed}");


        // }

        // builder.Services.AddSingleton(new Random(seed));
        // builder.Services.AddSingleton<ISkipStrategy, MyStrategy>();
        // builder.Services.AddSingleton<Semester>(injection =>
        // {
        //     var random = injection.GetRequiredService<Random>();
        //     var strategy = injection.GetRequiredService<ISkipStrategy>();
        //     var professors = ProfessorGenerator.Generate(random);
        //     return new Semester(professors, strategy);
        // });
        // builder.Services.AddHostedService<MyWorker>();
        // builder.Services.AddHostedService<KeyboardStopService>();
        // IHost host = builder.Build();
        // await host.RunAsync();


    }


}