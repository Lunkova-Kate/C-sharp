namespace MasterOfSkipping.StudentService.Contracts.History;

public sealed record DayStatusResponse(
    Guid SemesterId,
    int Day,
    int SavedLessonCount,
    bool IsCompleted);