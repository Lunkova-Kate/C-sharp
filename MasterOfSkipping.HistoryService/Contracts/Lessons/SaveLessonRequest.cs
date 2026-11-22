namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record SaveLessonRequest(
    string SubjectCode,
    bool Attended,
    bool WasAsked);
