namespace MasterOfSkipping.ProfessorService.Contracts.Student;

public sealed record StudentRequest(
    Guid SemesterId,
    int Day,
    bool Attended);