namespace MasterOfSkipping.ProfessorService.Contracts.History;

public sealed record QuestionResponse(
    string SubjectCode,
    bool WasAsked);