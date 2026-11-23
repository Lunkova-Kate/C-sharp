using MasterOfSkipping.HistoryService.Data.Entities;
using MasterOfSkipping.HistoryService.Contracts;
using MasterOfSkipping.HistoryService.Data;
using Microsoft.EntityFrameworkCore;
using MasterOfSkipping.HistoryService.Services;
namespace MasterOfSkipping.HistoryService.Endpoints;

public static class SemesterEndpoints
{
    public static void MapSemesterEndpoints(this WebApplication app)
    {
        app.MapPost("/semesters", async (CreateSemesterRequest request, HistoryDbContext dbContext, ProfessorSettingsGenerator generator) =>
        {
            int seed = request.Seed ?? Random.Shared.Next();
            Guid semesterId = Guid.NewGuid();
            List<SubjectSettingEntity> settings = generator.Generate(semesterId, seed);
            var newSemester = new SemesterEntity
            {
                Id = semesterId,
                Seed = seed
            };
            dbContext.Semesters.Add(newSemester);
            dbContext.SubjectSettings.AddRange(settings);
            await dbContext.SaveChangesAsync();
            string location = $"/semesters/{semesterId}";
            var response = new CreateSemesterResponse(semesterId, seed);
            return Results.Created(location, response);
        });

        app.MapGet("/semesters/unfinished", async (HistoryDbContext dbContext) =>
        {
            List<GetSemesterResponse> semesters = await dbContext.Semesters
                .AsNoTracking()
                .Where(semester =>
                    !semester.IsExpelled &&
                    semester.DaysCompleted < 100)
                .Select(semester => new GetSemesterResponse(
                    semester.Id,
                    semester.Seed,
                    semester.DaysCompleted,
                    semester.TotalEnjoyment,
                    semester.IsExpelled))
                .ToListAsync();

            return Results.Ok(semesters);
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

        app.MapGet("/semesters/{semesterId:guid}/settings/{subjectCode}", async (
            Guid semesterId,
            string subjectCode,
            HistoryDbContext dbContext) =>
        {
            string normalizedSubjectCode = subjectCode.Trim().ToUpperInvariant();

            SubjectSettingEntity? setting = await dbContext.SubjectSettings
                .AsNoTracking()
                .SingleOrDefaultAsync(subjectSetting =>
                    subjectSetting.SemesterId == semesterId &&
                    subjectSetting.SubjectCode == normalizedSubjectCode);

            if (setting is null)
            {
                return Results.NotFound();
            }

            var response = new ProfessorSettingResponse(
                setting.SubjectCode,
                setting.RuleCode,
                setting.DependencyACode,
                setting.DependencyBCode);

            return Results.Ok(response);
        });

        app.MapPost("/semesters/{semesterId:guid}/days/{day:int}", async (
            Guid semesterId,
            int day,
            HistoryDbContext dbContext) =>
        {
            if (day is < 1 or > 100)
            {
                return Results.BadRequest(new { Error = "Day must be between 1 and 100." });
            }

            SemesterEntity? semester = await dbContext.Semesters.FindAsync(semesterId);

            if (semester is null)
            {
                return Results.NotFound();
            }

            if (semester.IsExpelled)
            {
                return Results.Conflict(new { Error = "The student has already been expelled." });
            }

            if (day != semester.DaysCompleted + 1)
            {
                return Results.Conflict(new
                {
                    Error = $"Expected day {semester.DaysCompleted + 1}, but received day {day}."
                });
            }

            SemesterDayEntity? existingDay = await dbContext.SemesterDays.FindAsync(
                semesterId,
                (short)day);

            if (existingDay is not null)
            {
                return Results.Conflict(new { Error = "The day has already been started." });
            }

            var semesterDay = new SemesterDayEntity
            {
                SemesterId = semesterId,
                DayNumber = (short)day,
                TotalEnjoyment = semester.TotalEnjoyment,
                IsExpelled = false
            };

            dbContext.SemesterDays.Add(semesterDay);
            await dbContext.SaveChangesAsync();

            string location = $"/semesters/{semesterId}/days/{day}";
            var response = new StartDayResponse(
                semesterId,
                day,
                semesterDay.TotalEnjoyment,
                semesterDay.IsExpelled);

            return Results.Created(location, response);
        });

        app.MapGet("/semesters/{semesterId:guid}/days/{day:int}/status",
            async (
                Guid semesterId,
                int day,
                HistoryDbContext dbContext) =>
            {
                if (day is < 1 or > 100)
                {
                    return Results.BadRequest(new
                    {
                        Error = "Day must be between 1 and 100."
                    });
                }

                SemesterEntity? semester =
                    await dbContext.Semesters.FindAsync(semesterId);

                SemesterDayEntity? semesterDay =
                    await dbContext.SemesterDays.FindAsync(
                        semesterId,
                        (short)day);

                if (semester is null || semesterDay is null)
                {
                    return Results.NotFound();
                }

                int savedLessonCount =
                    await dbContext.Lessons.CountAsync(
                        lesson =>
                            lesson.SemesterId == semesterId &&
                            lesson.DayNumber == day);

                bool isCompleted =
                    semester.DaysCompleted >= day;

                var response = new DayStatusResponse(
                    semesterId,
                    day,
                    savedLessonCount,
                    isCompleted);

                return Results.Ok(response);
            });


        app.MapPut("/semesters/{semesterId:guid}/days/{day:int}", async (
            Guid semesterId,
            int day,
            CompleteDayRequest request,
            HistoryDbContext dbContext) =>
        {
            if (day is < 1 or > 100)
            {
                return Results.BadRequest(new { Error = "Day must be between 1 and 100." });
            }

            if (request.TotalEnjoyment < 0 ||
                request.IsExpelled && request.TotalEnjoyment != 0)
            {
                return Results.BadRequest(new
                {
                    Error = "Enjoyment cannot be negative and must be zero after expulsion."
                });
            }

            SemesterEntity? semester = await dbContext.Semesters.FindAsync(semesterId);
            SemesterDayEntity? semesterDay = await dbContext.SemesterDays.FindAsync(
                semesterId,
                (short)day);

            if (semester is null || semesterDay is null)
            {
                return Results.NotFound();
            }

            if (day != semester.DaysCompleted + 1)
            {
                return Results.Conflict(new { Error = "Days must be completed in order." });
            }

            int lessonCount = await dbContext.Lessons.CountAsync(lesson =>
                lesson.SemesterId == semesterId &&
                lesson.DayNumber == day);

            if (!request.IsExpelled && lessonCount != 6)
            {
                return Results.Conflict(new
                {
                    Error = $"A completed day must contain 6 lessons; found {lessonCount}."
                });
            }

            if (request.IsExpelled && lessonCount is < 1 or > 6)
            {
                return Results.Conflict(new
                {
                    Error = $"An expelled day must contain between 1 and 6 lessons; found {lessonCount}."
                });
            }

            semesterDay.TotalEnjoyment = request.TotalEnjoyment;
            semesterDay.IsExpelled = request.IsExpelled;

            semester.DaysCompleted = (short)day;
            semester.TotalEnjoyment = request.TotalEnjoyment;
            semester.IsExpelled = request.IsExpelled;

            await dbContext.SaveChangesAsync();

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
