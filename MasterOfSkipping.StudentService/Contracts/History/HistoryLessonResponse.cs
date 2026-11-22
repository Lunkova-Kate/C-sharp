namespace MasterOfSkipping.StudentService.Contracts.History;

public sealed record HistoryLessonResponse(
    int Day,
    string SubjectCode,
    bool Attended,
    bool? WasAsked);
