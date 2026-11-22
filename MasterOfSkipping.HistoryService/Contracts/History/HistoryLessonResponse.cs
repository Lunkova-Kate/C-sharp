namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record HistoryLessonResponse(
    int Day,
    string SubjectCode,
    bool Attended,
    bool? WasAsked);
