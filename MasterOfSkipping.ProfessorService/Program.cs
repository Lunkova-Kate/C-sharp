using MasterOfSkipping.ProfessorService.Clients;
using MasterOfSkipping.ProfessorService.Services;
using MasterOfSkipping.ProfessorService.Endpoints;

var builder = WebApplication.CreateBuilder(args);



string historyServiceUrl = builder.Configuration["HistoryService:BaseUrl"]
    ?? throw new InvalidOperationException(
        "HistoryService:BaseUrl was not configured.");

string subjectCode = builder.Configuration["Professor:SubjectCode"]
    ?? throw new InvalidOperationException(
        "Professor:SubjectCode was not configured.");



builder.Services.AddHttpClient("HistoryService", client =>
{
    client.BaseAddress = new Uri(historyServiceUrl);
});
builder.Services.AddScoped<HistoryClient>();
builder.Services.AddSingleton<ProfessorRule>();


builder.Services.AddSingleton<LessonResultPublisher>();
builder.Services.AddHostedService(serviceProvider =>
    serviceProvider.GetRequiredService<LessonResultPublisher>());

var app = builder.Build();

app.MapGet("/health", () => new
{
    Status = "ok",
    Service = "Professor",
    SubjectCode = subjectCode
});
app.MapProfessorEndpoints(subjectCode);


app.Run();
