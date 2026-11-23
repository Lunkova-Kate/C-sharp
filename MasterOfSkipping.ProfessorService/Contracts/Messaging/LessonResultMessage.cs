namespace MasterOfSkipping.ProfessorService.Contracts.Messaging;

public sealed record LessonResultMessage(
    Guid MessageId,
    Guid SemesterId,
    int Day,
    string SubjectCode,
    bool Attended,
    bool WasAsked);
