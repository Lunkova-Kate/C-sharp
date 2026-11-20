using MasterOfSkipping.HistoryService.Contracts;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/health", () => new
{
    Status = "ok",
    Service = "History"
});

app.MapPost("/semester", (CreateSemesterRequest request) => 
{
    int? seed = request.Seed ?? Random.Shared.Next();
    Guid semesterId = Guid.NewGuid();
    var response = new CreateSemesterResponse( semesterId, seed);
    return response;
} );



app.Run();
