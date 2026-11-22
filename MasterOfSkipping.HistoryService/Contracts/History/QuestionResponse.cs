namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record QuestionResponse(
    string SubjectCode,
    bool WasAsked);
