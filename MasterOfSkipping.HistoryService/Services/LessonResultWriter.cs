using MasterOfSkipping.HistoryService.Contracts.Messaging;
using MasterOfSkipping.HistoryService.Data;
using MasterOfSkipping.HistoryService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MasterOfSkipping.HistoryService.Services;

public sealed class LessonResultWriter
{
    private readonly HistoryDbContext _dbContext;

    public LessonResultWriter(HistoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveAsync(
        LessonResultMessage message,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (message.MessageId == Guid.Empty)
        {
            throw new InvalidOperationException(
                "MessageId cannot be empty.");
        }

        if (message.Day is < 1 or > 100)
        {
            throw new InvalidOperationException(
                "Day must be between 1 and 100.");
        }

        if (string.IsNullOrWhiteSpace(message.SubjectCode))
        {
            throw new InvalidOperationException(
                "SubjectCode is required.");
        }

        string subjectCode = message.SubjectCode
            .Trim()
            .ToUpperInvariant();

        LessonEntity? existingLesson = await _dbContext.Lessons
            .AsNoTracking()
            .Include(lesson => lesson.AttendanceDecision)
            .Include(lesson => lesson.QuestionResult)
            .SingleOrDefaultAsync(
                lesson =>
                    lesson.SemesterId == message.SemesterId &&
                    lesson.DayNumber == message.Day &&
                    lesson.SubjectCode == subjectCode,
                cancellationToken);

        if (existingLesson is not null)
        {
            bool isSameResult =
                existingLesson.AttendanceDecision?.Attended == message.Attended &&
                existingLesson.QuestionResult?.WasAsked == message.WasAsked;

            if (isSameResult)
            {
                return;
            }

            throw new InvalidOperationException(
                "A different result already exists for this lesson.");
        }

        SemesterEntity? semester = await _dbContext.Semesters
            .SingleOrDefaultAsync(
                semester => semester.Id == message.SemesterId,
                cancellationToken);

        if (semester is null)
        {
            throw new InvalidOperationException(
                "The semester was not found.");
        }

        if (semester.IsExpelled || message.Day <= semester.DaysCompleted)
        {
            throw new InvalidOperationException(
                "The semester day is already closed.");
        }

        if (message.Day != semester.DaysCompleted + 1)
        {
            throw new InvalidOperationException(
                $"Expected day {semester.DaysCompleted + 1}, " +
                $"but received day {message.Day}.");
        }

        bool dayExists = await _dbContext.SemesterDays.AnyAsync(
            day =>
                day.SemesterId == message.SemesterId &&
                day.DayNumber == message.Day,
            cancellationToken);

        if (!dayExists)
        {
            throw new InvalidOperationException(
                "The semester day was not found.");
        }

        bool subjectExists = await _dbContext.SubjectSettings.AnyAsync(
            setting =>
                setting.SemesterId == message.SemesterId &&
                setting.SubjectCode == subjectCode,
            cancellationToken);

        if (!subjectExists)
        {
            throw new InvalidOperationException(
                "The subject setting was not found.");
        }

        var lesson = new LessonEntity
        {
            SemesterId = message.SemesterId,
            DayNumber = (short)message.Day,
            SubjectCode = subjectCode
        };

        var attendance = new AttendDecisionEntity
        {
            SemesterId = message.SemesterId,
            DayNumber = (short)message.Day,
            SubjectCode = subjectCode,
            Attended = message.Attended
        };

        var question = new QuestionResultEntity
        {
            SemesterId = message.SemesterId,
            DayNumber = (short)message.Day,
            SubjectCode = subjectCode,
            WasAsked = message.WasAsked
        };

        _dbContext.Lessons.Add(lesson);
        _dbContext.AttendDecision.Add(attendance);
        _dbContext.QuestionResults.Add(question);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
