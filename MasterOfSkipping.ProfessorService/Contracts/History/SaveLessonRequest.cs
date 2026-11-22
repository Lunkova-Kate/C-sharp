namespace MasterOfSkipping.ProfessorService.Contracts.History;

public sealed record SaveLessonRequest(
    string SubjectCode,
    bool Attended,
    bool WasAsked);