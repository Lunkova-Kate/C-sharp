using MasterOfSkipping.HistoryService.Contracts;
using MasterOfSkipping.HistoryService.Data;
using MasterOfSkipping.HistoryService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasterOfSkipping.HistoryService.Endpoints;

public static class HistoryEndpoints
{
    public static void MapHistoryEndpoints(this WebApplication app)
    {
        app.MapPost("/semesters/{semesterId:guid}/days/{day:int}/lessons", async (
            Guid semesterId,
            int day,
            SaveLessonRequest request,
            HistoryDbContext dbContext) =>
        {
            if (day is < 1 or > 100)
            {
                return Results.BadRequest(new { Error = "Day must be between 1 and 100." });
            }

            if (string.IsNullOrWhiteSpace(request.SubjectCode))
            {
                return Results.BadRequest(new { Error = "SubjectCode is required." });
            }

            string subjectCode = request.SubjectCode.Trim().ToUpperInvariant();

            SemesterEntity? semester = await dbContext.Semesters.FindAsync(semesterId);

            if (semester is null)
            {
                return Results.NotFound(new { Error = "The semester was not found." });
            }

            if (semester.IsExpelled || day <= semester.DaysCompleted)
            {
                return Results.Conflict(new { Error = "The semester day is already closed." });
            }

            if (day != semester.DaysCompleted + 1)
            {
                return Results.Conflict(new
                {
                    Error = $"Expected day {semester.DaysCompleted + 1}, but received day {day}."
                });
            }

            SemesterDayEntity? semesterDay = await dbContext.SemesterDays.FindAsync(
                semesterId,
                (short)day);

            if (semesterDay is null)
            {
                return Results.NotFound(new { Error = "The semester day was not found." });
            }

            SubjectSettingEntity? subjectSetting = await dbContext.SubjectSettings.FindAsync(
                semesterId,
                subjectCode);

            if (subjectSetting is null)
            {
                return Results.NotFound(new { Error = "The subject setting was not found." });
            }

            LessonEntity? existingLesson = await dbContext.Lessons.FindAsync(
                semesterId,
                (short)day,
                subjectCode);

            if (existingLesson is not null)
            {
                return Results.Conflict(new { Error = "The lesson result already exists." });
            }

            var lesson = new LessonEntity
            {
                SemesterId = semesterId,
                DayNumber = (short)day,
                SubjectCode = subjectCode
            };

            var attendance = new AttendDecisionEntity
            {
                SemesterId = semesterId,
                DayNumber = (short)day,
                SubjectCode = subjectCode,
                Attended = request.Attended
            };

            var question = new QuestionResultEntity
            {
                SemesterId = semesterId,
                DayNumber = (short)day,
                SubjectCode = subjectCode,
                WasAsked = request.WasAsked
            };

            dbContext.Lessons.Add(lesson);
            dbContext.AttendDecision.Add(attendance);
            dbContext.QuestionResults.Add(question);
            await dbContext.SaveChangesAsync();

            string location = $"/semesters/{semesterId}/days/{day}/lessons/{subjectCode}";
            var response = new SaveLessonResponse(
                semesterId,
                day,
                subjectCode,
                request.Attended,
                request.WasAsked);

            return Results.Created(location, response);
        });

        app.MapGet("/semesters/{semesterId:guid}/days/{day:int}/questions", async (
            Guid semesterId,
            int day,
            HistoryDbContext dbContext) =>
        {
            if (day is < 1 or > 100)
            {
                return Results.BadRequest(new { Error = "Day must be between 1 and 100." });
            }

            SemesterEntity? semester = await dbContext.Semesters
                .AsNoTracking()
                .SingleOrDefaultAsync(entity => entity.Id == semesterId);

            if (semester is null)
            {
                return Results.NotFound();
            }

            if (day > semester.DaysCompleted)
            {
                return Results.Conflict(new { Error = "The requested day is not completed." });
            }

            List<QuestionResponse> questions = await dbContext.QuestionResults
                .AsNoTracking()
                .Where(question =>
                    question.SemesterId == semesterId &&
                    question.DayNumber == day)
                .OrderBy(question => question.SubjectCode)
                .Select(question => new QuestionResponse(
                    question.SubjectCode,
                    question.WasAsked))
                .ToListAsync();

            return Results.Ok(questions);
        });

        app.MapGet("/semesters/{semesterId:guid}/history", async (
            Guid semesterId,
            int beforeDay,
            HistoryDbContext dbContext) =>
        {
            if (beforeDay is < 1 or > 101)
            {
                return Results.BadRequest(new { Error = "beforeDay must be between 1 and 101." });
            }

            SemesterEntity? semester = await dbContext.Semesters
                .AsNoTracking()
                .SingleOrDefaultAsync(entity => entity.Id == semesterId);

            if (semester is null)
            {
                return Results.NotFound();
            }

            int lastVisibleDay = Math.Min(beforeDay - 1, semester.DaysCompleted);

            List<HistoryLessonResponse> history = await dbContext.Lessons
                .AsNoTracking()
                .Where(lesson =>
                    lesson.SemesterId == semesterId &&
                    lesson.DayNumber <= lastVisibleDay)
                .OrderBy(lesson => lesson.DayNumber)
                .ThenBy(lesson => lesson.SubjectCode)
                .Select(lesson => new HistoryLessonResponse(
                    lesson.DayNumber,
                    lesson.SubjectCode,
                    lesson.AttendanceDecision!.Attended,
                    lesson.AttendanceDecision.Attended
                        ? lesson.QuestionResult!.WasAsked
                        : null))
                .ToListAsync();

            return Results.Ok(history);
        });
    }
}
