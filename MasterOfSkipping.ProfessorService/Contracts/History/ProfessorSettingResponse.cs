namespace MasterOfSkipping.ProfessorService.Contracts.History;

public sealed record ProfessorSettingResponse(
    string SubjectCode,
    string RuleCode,
    string? DependencyA,
    string? DependencyB);