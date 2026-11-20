using MasterOfSkipping.HistoryService.Contracts;



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => new
{
    Status = "ok",
    Service = "History"
});

app.MapPost("/semesters", (CreateSemesterRequest request) => 
{
    int seed = request.Seed ?? Random.Shared.Next();
    Guid semesterId = Guid.NewGuid();
    var response = new CreateSemesterResponse( semesterId, seed);
    var location = $"/semesters/{semesterId}";
    return Results.Created(location, response);
} );

app.MapGet("/semesters/{semesterId:guid}", (Guid SemesterId) => new
{
    status = true,
    semesterid = SemesterId
});


app.Run();
