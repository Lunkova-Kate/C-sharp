namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record SaveLessonResponse(
    Guid SemesterId,
    int Day,
    string SubjectCode,
    bool Attended,
    bool WasAsked);
