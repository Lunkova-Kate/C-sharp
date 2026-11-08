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



        // int seed = 123;
        // Random random = new Random(seed);
        // Professor[] professors = ProfessorGenerator.Generate(random);
        // MyStrategy strategy = new MyStrategy();
        // Semester semester = new Semester(professors, strategy);

        using var _db = new AppOnDBContext(
            new DbContextOptionsBuilder<AppOnDBContext>()
                .UseNpgsql(connectionSetting)
                .Options);
        {

            SemesterRepo semesterRepo = new SemesterRepo(_db);
            List<SemesterEntity> unfinishedSemesters = await semesterRepo.GetUnfinishedSemestersAsync();


            var lastSemester = await semesterRepo.GetSemesterByIdAsync(unfinishedSemesters.Last().Id);
            Console.WriteLine($"DaysCount {lastSemester.SemesterDays.Count}, SUbjectCount {lastSemester.SubjectSettings.Count()}, LessonsCount {lastSemester.SemesterDays.Sum(day => day.Lessons.Count)}");
            Random lastSemerterRandom = new Random(lastSemester.Seed);
            Professor[] lastProffesor = ProfessorGenerator.Generate(lastSemerterRandom);
            MyStrategy laststSemestrStrategy = new MyStrategy();
            Semester restoreSemester = new Semester(lastProffesor, laststSemestrStrategy);

            for (var i = 0; i < lastSemester.DaysCompleted; ++i)
            {
                restoreSemester.ProcessDay();
            }
            // DayResult nextDay = restoreSemester.ProcessDay();
            // await semesterRepo.SaveDayAsync(lastSemester.Id, nextDay);

            Console.WriteLine(
                $"Restored: Days={restoreSemester.DaysElapsed}, " +
                $"Enjoyment={restoreSemester.TotalEnjoyment}");

            Console.WriteLine(
                $"Database: Days={lastSemester.DaysCompleted}, " +
                $"Enjoyment={lastSemester.TotalEnjoyment}");



        }


    }


}