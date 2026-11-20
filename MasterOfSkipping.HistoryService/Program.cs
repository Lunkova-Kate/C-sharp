using MasterOfSkipping.HistoryService.Contracts;
using Microsoft.EntityFrameworkCore;
using MasterOfSkipping.HistoryService.Data;
using MasterOfSkipping.HistoryService.Data.Entities;
using MasterOfSkipping.HistoryService.Endpoints;



var builder = WebApplication.CreateBuilder(args);
string connectionString = builder.Configuration.GetConnectionString("HistoryDatabase") ?? throw new InvalidOperationException("Connection string 'HistoryDatabase' was not found.");
builder.Services.AddDbContext<HistoryDbContext> (option =>
{
    option.UseNpgsql(connectionString);
} );
var app = builder.Build();


app.MapSemesterEndpoints();


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
