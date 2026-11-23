using MasterOfSkipping.StudentService.Clients;
using MasterOfSkipping.StudentService.Core;
using MasterOfSkipping.StudentService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

if (args.Length == 0)
{
    Console.WriteLine("Specify a mode: new or resume.");
    return;
}

string mode = args[0].ToLowerInvariant();

if (mode is not "new" and not "resume")
{
    Console.WriteLine(
        $"Unknown mode '{args[0]}'. Specify either new or resume.");
    return;
}

string[] configurationArgs = args.Skip(1).ToArray();

HostApplicationBuilder builder =
    Host.CreateApplicationBuilder(
        new HostApplicationBuilderSettings
        {
            Args = configurationArgs,
            ContentRootPath = AppContext.BaseDirectory
        });

string historyServiceUrl = builder.Configuration["HistoryService:BaseUrl"]
    ?? throw new InvalidOperationException(
        "HistoryService:BaseUrl was not configured.");

builder.Services.AddHttpClient(
    "HistoryService",
    httpClient => httpClient.BaseAddress = new Uri(historyServiceUrl));

IConfigurationSection professorsSection =
    builder.Configuration.GetSection("ProfessorServices");

var configuredSubjects = new HashSet<string>(StringComparer.Ordinal);

foreach (IConfigurationSection professor in professorsSection.GetChildren())
{
    string subjectCode = professor.Key;
    string professorUrl = professor.Value
        ?? throw new InvalidOperationException(
            $"URL for {subjectCode} was not configured.");

    builder.Services.AddHttpClient(
        subjectCode,
        httpClient => httpClient.BaseAddress = new Uri(professorUrl));

    configuredSubjects.Add(subjectCode);
}

string[] missingSubjects = SubjectCatalog.Codes
    .Where(subjectCode => !configuredSubjects.Contains(subjectCode))
    .ToArray();

if (missingSubjects.Length > 0)
{
    throw new InvalidOperationException(
        $"Professor URLs were not configured for: {string.Join(", ", missingSubjects)}.");
}

builder.Services.AddSingleton<HistoryClient>();
builder.Services.AddSingleton<ProfessorClient>();
builder.Services.AddSingleton<StudentStrategy>();
builder.Services.AddSingleton<StudentRunner>();

using IHost host = builder.Build();

StudentRunner runner = host.Services.GetRequiredService<StudentRunner>();

try
{
    switch (mode)
    {
        case "new":
            int? seed = builder.Configuration.GetValue<int?>("Semester:Seed");
            await runner.RunNewAsync(seed);
            break;

        case "resume":
            await runner.ResumeAsync();
            break;
    }
}
catch (HttpRequestException exception)
{
    Console.Error.WriteLine($"HTTP request failed: {exception.Message}");
    Environment.ExitCode = 1;
}
