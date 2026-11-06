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
        var options = new DbContextOptionsBuilder<AppOnDBContext>()
        .UseNpgsql(connectionSetting);

        using (var db = new AppOnDBContext(options.Options))
        {

            // var rules = await db.Rules.ToListAsync();
            // foreach (var rule in rules)
            // {
            //     Console.WriteLine($"Code: {rule.Code}, Name: {rule.Name}");
            // }

            // var subjects = db.Subjects.OrderBy(s => s.LessonOrder).ToList();
            // foreach (var subject in subjects)
            // {
            //     Console.WriteLine($"Code: {subject.Code}, Name: {subject.Name}, Lesson Order: {subject.LessonOrder}");
            // }
            // var semesters = await db.Semesters.ToListAsync();
            // Console.WriteLine($"Семестров в базе: {semesters.Count}");
            // var semestr = new SemesterEntity
            // {
            //     Id = Guid.NewGuid(),
            //     Seed = 12345,
            // };
            // db.Semesters.Add(semestr);
            // await db.SaveChangesAsync();
            Guid semestrId = Guid.Parse("f606c904-300b-4d04-8468-d187791901bc");
            var savedSemester =  db.Semesters.SingleOrDefault(seme => seme.Id == semestrId);
            Console.WriteLine($"Семестр с ID {semestrId} найден: {savedSemester.Id }, Seed: {savedSemester.Seed}, DaysCompleted: {savedSemester.DaysCompleted}, TotalEnjoyment: {savedSemester.TotalEnjoyment}, IsExpelled: {savedSemester.IsExpelled}");
        }




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
