using MasterOfSkipping.HistoryService.Contracts;
using Microsoft.EntityFrameworkCore;
using MasterOfSkipping.HistoryService.Data;
using MasterOfSkipping.HistoryService.Data.Entities;



var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("HistoryDatabase") ?? throw new InvalidOperationException("Connection string 'HistoryDatabase' was not found.");
builder.Services.AddDbContext<HistoryDbContext> (option =>
{
    option.UseNpgsql(connectionString);
} );
var app = builder.Build();


app.MapPost("/semesters", async (CreateSemesterRequest request, HistoryDbContext dbContext) =>
{
    int seed = request.Seed ?? Random.Shared.Next();
    Guid semesterId = Guid.NewGuid();
    var newSemester = new SemesterEntity
    {
        Id = semesterId,
        Seed = seed
    };
    dbContext.Semesters.Add(newSemester);
    await dbContext.SaveChangesAsync();
    string location = $"/semesters/{semesterId}";
    var response = new CreateSemesterResponse(semesterId, seed);
    return Results.Created(location, response);
   
    // return Results.Created(located, response)
    
});


app.MapGet("/health/database", async (HistoryDbContext dbContext) => new
{
     canConnect = await dbContext.Database.CanConnectAsync()
    
});

app.MapGet("/health", () => new
{
    Status = "ok",
    Service = "History"
});



app.Run();
