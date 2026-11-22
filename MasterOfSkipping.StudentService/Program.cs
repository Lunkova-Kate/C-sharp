using MasterOfSkipping.StudentService.Clients;
using MasterOfSkipping.StudentService.Core;
using MasterOfSkipping.StudentService.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder =
    Host.CreateApplicationBuilder(
        new HostApplicationBuilderSettings
        {
            Args = args,
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

int? seed = builder.Configuration.GetValue<int?>("Semester:Seed");
StudentRunner runner = host.Services.GetRequiredService<StudentRunner>();

try
{
    await runner.RunAsync(seed);
}
catch (HttpRequestException exception)
{
    Console.Error.WriteLine($"HTTP request failed: {exception.Message}");
    Environment.ExitCode = 1;
}
