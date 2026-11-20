using MasterOfSkipping.HistoryService.Data.Entities;
using MasterOfSkipping.HistoryService.Contracts;
using MasterOfSkipping.HistoryService.Data;

namespace MasterOfSkipping.HistoryService.Endpoints;

public static class SemesterEndpoints
{
    public static void MapSemesterEndpoints(this WebApplication app)
    {
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
        });



        app.MapGet("/semesters/{semesterId:guid}", async (Guid semesterId, HistoryDbContext dbContext) =>
        {
            SemesterEntity? semester = await dbContext.FindAsync<SemesterEntity>(semesterId);

            if (semester is null)
            {
                return Results.NotFound();
            }

            var response = new GetSemesterResponse(
                semester.Id,
                semester.Seed,
                semester.DaysCompleted,
                semester.TotalEnjoyment,
                semester.IsExpelled);

            return Results.Ok(response);
        });
    }



}
