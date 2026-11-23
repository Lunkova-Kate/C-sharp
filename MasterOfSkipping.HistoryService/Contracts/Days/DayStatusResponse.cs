namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record DayStatusResponse(
    Guid SemesterId,
    int Day,
    int SavedLessonCount, 
    bool IsCompleted); 