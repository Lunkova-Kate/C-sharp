namespace MasterOfSkipping.ProfessorService.Contracts.Student;

public sealed record StudentResponse(
    string SubjectCode,
    bool WasAsked);