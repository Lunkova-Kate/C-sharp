namespace MasterOfSkipping.HistoryService.Contracts;

public sealed record ProfessorSettingResponse(
    string SubjectCode,
    string RuleCode,
    string? DependencyA,
    string? DependencyB);
