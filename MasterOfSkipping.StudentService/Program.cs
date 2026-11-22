using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
string historyServiceUrl = builder.Configuration["HistoryService:BaseUrl"]
    ?? throw new InvalidOperationException(
        "HistoryService:BaseUrl was not configured.");

string calculusProfessorUrl = builder.Configuration["ProfessorServices:CALCULUS"]
?? throw new InvalidOperationException(
"CALCULUS professor URL was not configured.");

Console.WriteLine(historyServiceUrl);
Console.WriteLine(calculusProfessorUrl);